using backend.DTOs;
using backend.Model;
using Microsoft.EntityFrameworkCore;

namespace backend.Services {
    public class JobService(AppDbContext appDbContext) {
        public async Task<List<JobSummaryDto>> GetTopNJobsAsync(int n) {
            var jobs = await appDbContext.Jobs
                .OrderByDescending(j => j.PostedAt)
                .Take(n)
                .ToListAsync();
            
            return jobs.Select(JobSummaryDto.FromJob).ToList();
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

            // Sorting
            query = filter.SortBy switch {
                JobSortBy.Newest          => query.OrderByDescending(j => j.PostedAt),
                JobSortBy.Oldest          => query.OrderBy(j => j.PostedAt),
                JobSortBy.HighestBudget   => query.OrderByDescending(j => j.Budget),
                JobSortBy.MostApplicants  => query.OrderByDescending(j => j.Applications.Count),
                JobSortBy.ClosestDeadline => query.OrderBy(j => j.Deadline),
                _                         => query.OrderBy(j => j.Deadline)
            };

            var jobs = await query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PaginatedResponse<JobSummaryDto> {
                Items = jobs.Select(JobSummaryDto.FromJob).ToList(),
                TotalCount = jobs.Count(),
            };
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
