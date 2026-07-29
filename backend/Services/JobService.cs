using backend.DTOs;
using backend.Model;
using Microsoft.EntityFrameworkCore;

namespace backend.Services {
    public class JobService(AppDbContext appDbContext) {
        public async Task<List<JobSummaryDto>> GetTopNJobsAsync(int n) {
            var jobs = await appDbContext.Jobs
                .Where(j => j.JobStatus == JobStatus.Approved && j.Deadline >= DateOnly.FromDateTime(DateTime.UtcNow))
                .OrderByDescending(j => j.PostedAt)
                .Take(n)
                .ToListAsync();

            return jobs.Select(JobSummaryDto.FromJob).ToList();
        }

        public async Task<JobDto?> GetJobByIdAsync(int id) {
            var job = await appDbContext.Jobs
                    .Include(j => j.Client)
                    .Include(j => j.Applications)
                    .Include(j => j.Attachments)
                    .Include(j => j.Categories)
                        .ThenInclude(jc => jc.Category)
                    .Include(j => j.Tags)
                        .ThenInclude(jt => jt.Tag)
                    .Include(j => j.Skills)
                        .ThenInclude(js => js.Skill)
                    .FirstOrDefaultAsync(j => j.Id == id);

            return job == null ? null : JobDto.FromJob(job);
        }

        public async Task<PaginatedResponse<JobSummaryDto>> GetJobsAsync(JobFilterDto filter) {
            var query = appDbContext.Jobs.AsNoTracking().AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(filter.Search)) {
                query = query.Where(j =>
                    j.Title.Contains(filter.Search) ||
                    j.Description.Contains(filter.Search));
            }

            // Category
            if (filter.CategoryId.HasValue) {
                query = query.Where(j => j.Categories.Any(c => c.CategoryId == filter.CategoryId.Value));
            }

            // Status — public listings only show approved jobs unless a specific status is requested
            if (filter.Status.HasValue) {
                query = query.Where(j => j.JobStatus == filter.Status.Value);
            } else if (!filter.ClientId.HasValue) {
                var today = DateOnly.FromDateTime(DateTime.UtcNow);
                query = query.Where(j => j.JobStatus == JobStatus.Approved && j.Deadline >= today);
            }

            // Skills
            if (filter.SkillIds?.Any() == true) {
                query = query.Where(j => j.Skills.Any(s => filter.SkillIds.Contains(s.SkillId)));
            }

            // Budget
            if (filter.MinBudget.HasValue) {
                query = query.Where(j => j.Budget >= filter.MinBudget.Value);
            }

            if (filter.MaxBudget.HasValue) {
                query = query.Where(j => j.Budget <= filter.MaxBudget.Value);
            }

            // certain Client
            if (filter.ClientId.HasValue) {
                query = query.Where(j => j.ClientId == filter.ClientId.Value);
            }

            // Sorting
            query = filter.SortBy switch {
                JobSortBy.Newest => query.OrderByDescending(j => j.PostedAt),
                JobSortBy.Oldest => query.OrderBy(j => j.PostedAt),
                JobSortBy.HighestBudget => query.OrderByDescending(j => j.Budget),
                JobSortBy.MostApplicants => query.OrderByDescending(j => j.Applications.Count),
                JobSortBy.ClosestDeadline => query.OrderBy(j => j.Deadline),
                _ => query.OrderBy(j => j.Deadline)
            };

            var jobs = await query
                        .Skip((filter.Page - 1) * filter.PageSize)
                        .Include(j => j.Categories)
                            .ThenInclude(jc => jc.Category)
                        .Include(j => j.Tags)
                            .ThenInclude(jt => jt.Tag)
                        .Include(j => j.Applications)
                        .Take(filter.PageSize)
                        .ToListAsync();

            return new PaginatedResponse<JobSummaryDto> {
                Items = jobs.Select(JobSummaryDto.FromJob).ToList(),
                TotalCount = jobs.Count(),
            };
        }

        private async Task CheckIdsAsync<TEntity>(
                IQueryable<TEntity> dbSet, 
                List<int> ids, 
                string entityName, 
                int minAllowed, 
                int maxAllowed
        ) where TEntity : class {

            var distinctIds = ids.Distinct().ToList();

            if (distinctIds.Count < minAllowed)
                throw new ArgumentException($"At least {minAllowed} valid {entityName} IDs are required.");

            if (distinctIds.Count > maxAllowed)
                throw new ArgumentException($"A maximum of {maxAllowed} {entityName} IDs are allowed.");

            var existingCount = await dbSet.CountAsync(e => distinctIds.Contains(EF.Property<int>(e, "Id")));

            if (existingCount != distinctIds.Count)
                throw new ArgumentException($"One or more {entityName} IDs are invalid.");
        }

        public async Task CreateJobAsync(JobCreateDto newJob, int clientId) {
            var categoryIds = newJob.CategoryIds.Distinct().ToList();
            var skillIds = newJob.SkillIds.Distinct().ToList();
            var tagIds = newJob.TagIds.Distinct().ToList();

            await CheckIdsAsync(appDbContext.Categories, categoryIds, "category", 1, 3);
            await CheckIdsAsync(appDbContext.Skills, skillIds, "skill", 1, 10);
            await CheckIdsAsync(appDbContext.Tags, tagIds, "tag", 0, 10);

            var job = new Job {
                Title = newJob.Title,
                Description = newJob.Description,
                Budget = (int)newJob.Budget,
                Deadline = newJob.Deadline,
                PostedAt = DateTime.UtcNow,
                JobStatus = JobStatus.Pending,
                ClientId = clientId
            };

            foreach (var categoryId in categoryIds)
                job.Categories.Add(new JobCategory { CategoryId = categoryId });

            foreach (var skillId in skillIds)
                job.Skills.Add(new JobSkill { SkillId = skillId });

            foreach (var tagId in tagIds)
                job.Tags.Add(new JobTag { TagId = tagId });

            appDbContext.Jobs.Add(job);
            await appDbContext.SaveChangesAsync();
        }

        public async Task UpdateJobAsync(int jobId, JobUpdateDto updatedJob, int clientId) {
            var job = await appDbContext.Jobs
                .Include(j => j.Categories)
                .Include(j => j.Skills)
                .Include(j => j.Tags)
                .FirstOrDefaultAsync(j => j.Id == jobId);

            if (job == null)
                throw new KeyNotFoundException("Job not found.");

            if (job.ClientId != clientId)
                throw new ArgumentException($"You are not allowed to edit this job. {job.ClientId}");

            if (job.JobStatus is not (JobStatus.Pending or JobStatus.Approved))
                throw new InvalidOperationException($"Jobs with status '{job.JobStatus}' cannot be edited.");

            var categoryIds = updatedJob.CategoryIds.Distinct().ToList();
            var skillIds = updatedJob.SkillIds.Distinct().ToList();
            var tagIds = updatedJob.TagIds.Distinct().ToList();

            await CheckIdsAsync(appDbContext.Categories, categoryIds, "category", 1, 3);
            await CheckIdsAsync(appDbContext.Skills, skillIds, "skill", 1, 10);
            await CheckIdsAsync(appDbContext.Tags, tagIds, "tag", 0, 10);

            job.Title = updatedJob.Title;
            job.Description = updatedJob.Description;
            job.Budget = (int)updatedJob.Budget;
            job.Deadline = updatedJob.Deadline;

            job.Categories.Clear();
            foreach (var categoryId in categoryIds)
                job.Categories.Add(new JobCategory { CategoryId = categoryId });

            job.Skills.Clear();
            foreach (var skillId in skillIds)
                job.Skills.Add(new JobSkill { SkillId = skillId });

            job.Tags.Clear();
            foreach (var tagId in tagIds)
                job.Tags.Add(new JobTag { TagId = tagId });

            await appDbContext.SaveChangesAsync();
        }

        public async Task DeleteJobAsync(int jobId, int clientId) {
            var job = await appDbContext.Jobs
                .FirstOrDefaultAsync(j => j.Id == jobId);

            if (job == null)
                throw new KeyNotFoundException("Job not found.");

            if (job.ClientId != clientId)
                throw new ArgumentException("You are not allowed to delete this job.");

            if (job.JobStatus is not (JobStatus.Pending or JobStatus.Rejected))
                throw new InvalidOperationException($"Jobs with status '{job.JobStatus}' cannot be deleted.");

            appDbContext.Jobs.Remove(job);

            await appDbContext.SaveChangesAsync();
        }

        public async Task<List<JobSummaryDto>> SearchJobsAsync(string searchTerm) {
            var jobs = await appDbContext.Jobs
                .Where(j => j.JobStatus == JobStatus.Approved
                         && j.Deadline >= DateOnly.FromDateTime(DateTime.UtcNow)
                         && (j.Title.Contains(searchTerm) || j.Description.Contains(searchTerm)))
                .ToListAsync();

            return jobs.Select(JobSummaryDto.FromJob).ToList();
        }
    }

    public enum JobSortBy {
        Newest,
        Oldest,
        HighestBudget,
        MostApplicants,
        ClosestDeadline
    }
}
