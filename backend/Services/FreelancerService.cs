using backend.model;
using Microsoft.EntityFrameworkCore;

namespace backend.Services {
    public class FreelancerService(AppDbContext appDbContext) {
        public async Task<List<Freelancer>> GetTopNFreelancersAsync(int n) {
            return await appDbContext.Freelancers
                .OrderByDescending(f => f.Applications.Count())
                .Take(n)
                .ToListAsync();
                //.Average(j => (double?)j.Job.Review.Rate) ?? 0)
        }
    }
}
