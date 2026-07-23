using backend.Dtos;
using backend.model;
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
    }
}
