using backend.Dtos;
using backend.Model;
using Microsoft.EntityFrameworkCore;

namespace backend.Services {
    public class FreelancerService(AppDbContext appDbContext) {
        public async Task<List<FreelancerSummaryDto>> GetTopNFreelancersAsync(int n) {
            var freelancers = await appDbContext.Freelancers
                            .OrderByDescending(f => f.Applications.Count())
                            .Take(n)
                            .ToListAsync();

            return freelancers.Select(FreelancerSummaryDto.FromFreelancer).ToList();
        }
    }
}
