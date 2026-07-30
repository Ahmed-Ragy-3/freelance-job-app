using backend.Model;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories
{
    public class FreelancerRepository : IFreelancerRepository
    {
        private readonly AppDbContext _context;

        public FreelancerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Freelancer?> GetByUserIdAsync(int userId)
        {
            return await _context.Freelancers
                .Include(f => f.User)
                .FirstOrDefaultAsync(f => f.UserId == userId);
        }

        public async Task<List<FreelancerSkill>> GetFreelancerSkillsAsync(int userId)
        {
            return await _context.Set<FreelancerSkill>()
                .Include(fs => fs.Skill)
                .Where(fs => fs.FreelancerId == userId)
                .ToListAsync();
        }

        public async Task<decimal> GetAverageRatingAsync(int userId)
        {
            var ratings = await _context.Reviews
                .Where(r => r.RevieweeId == userId)
                .Select(r => (decimal?)r.Rate)
                .ToListAsync();

            if (!ratings.Any() || ratings.All(r => !r.HasValue)) return 0;
            return Math.Round(ratings.Where(r => r.HasValue).Average(r => r!.Value), 2);
        }

        public async Task<bool> SkillsExistAsync(IEnumerable<int> skillIds)
        {
            var existingCount = await _context.Set<Skill>()
                .Where(s => skillIds.Contains(s.Id))
                .CountAsync();

            return existingCount == skillIds.Distinct().Count();
        }

        public async Task UpdateProfileAsync(Freelancer freelancer, List<FreelancerSkill>? newSkills)
        {
            _context.Freelancers.Update(freelancer);

            if (newSkills != null)
            {
                var existingSkills = await _context.Set<FreelancerSkill>()
                    .Where(fs => fs.FreelancerId == freelancer.UserId)
                    .ToListAsync();

                _context.Set<FreelancerSkill>().RemoveRange(existingSkills);
                await _context.Set<FreelancerSkill>().AddRangeAsync(newSkills);
            }

            await _context.SaveChangesAsync();
        }
    }
}
