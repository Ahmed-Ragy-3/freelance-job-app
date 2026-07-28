using backend.DTOs;
using backend.Model;
using Microsoft.EntityFrameworkCore;

namespace backend.Services {
    public class JobService(AppDbContext appDbContext) {
        //public async 
        public async Task<List<JobSummaryDto>> GetTopNJobsAsync(int n) {
            var jobs = await appDbContext.Jobs
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

            // Status
            if (filter.Status.HasValue) {
                query = query.Where(j => j.JobStatus == filter.Status.Value);
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

        public async Task CreateJobAsync(JobCreateDto newJob, int clientId) {
            // Remove duplicate IDs
            var categoryIds = newJob.CategoryIds.Distinct().ToList();
            var skillIds = newJob.SkillIds.Distinct().ToList();
            var tagIds = newJob.TagIds.Distinct().ToList();

            // Validate categories
            var existingCategoryIds = await appDbContext.Categories
                .Where(c => categoryIds.Contains(c.Id))
                .Select(c => c.Id)
                .ToListAsync();

            if (existingCategoryIds.Count != categoryIds.Count)
                throw new ArgumentException("One or more category IDs are invalid.");

            // Validate skills
            var existingSkillIds = await appDbContext.Skills
                .Where(s => skillIds.Contains(s.Id))
                .Select(s => s.Id)
                .ToListAsync();

            if (existingSkillIds.Count != skillIds.Count)
                throw new ArgumentException("One or more skill IDs are invalid.");

            // Validate tags
            var existingTagIds = await appDbContext.Tags
                .Where(t => tagIds.Contains(t.Id))
                .Select(t => t.Id)
                .ToListAsync();

            if (existingTagIds.Count != tagIds.Count)
                throw new ArgumentException("One or more tag IDs are invalid.");

            var job = new Job {
                Title = newJob.Title,
                Description = newJob.Description,
                Budget = (int)newJob.Budget,
                Deadline = newJob.Deadline,
                PostedAt = DateTime.UtcNow,
                JobStatus = JobStatus.Pending,
                ClientId = clientId
            };

            foreach (var categoryId in categoryIds) {
                job.Categories.Add(new JobCategory { CategoryId = categoryId });
            }

            foreach (var skillId in skillIds) {
                job.Skills.Add(new JobSkill { SkillId = skillId });
            }

            foreach (var tagId in tagIds) {
                job.Tags.Add(new JobTag { TagId = tagId });
            }

            appDbContext.Jobs.Add(job);

            await appDbContext.SaveChangesAsync();
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
