using backend.DTOs;
using backend.Model;
using Microsoft.EntityFrameworkCore;

namespace backend.Services {
    public class FreelancerService(AppDbContext appDbContext) {
        public async Task<List<FreelancerSummaryDto>> GetTopNFreelancersAsync(int n) {
            var freelancers = await appDbContext.Freelancers
                            .Include(f => f.User)
                            .Include(f => f.Applications)
                                .ThenInclude(a => a.Job)
                                .ThenInclude(j => j.Review)
                            .Include(f => f.FreelancerSkills)
                                .ThenInclude(fs => fs.Skill)
                            .OrderByDescending(f => f.Applications.Count)
                            .Take(n)
                            .ToListAsync();

            return freelancers.Select(FreelancerSummaryDto.FromFreelancer).ToList();
        }
    }
}
