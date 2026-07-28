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
            var reviews = await _context.Set<Application>()
                .Where(a => a.FreelancerId == userId && a.AppStatus == AppStatus.Accepted)
                .Select(a => a.Job.Review)
                .Where(r => r != null)
                .ToListAsync();

            if (!reviews.Any()) return 0;
            return (decimal)reviews.Average(r => r.Rate);
        }

        public async Task<bool> SkillsExistAsync(IEnumerable<int> skillIds)
        {
            var existingCount = await _context.Set<Skill>()
                .Where(s => skillIds.Contains(s.Id))
                .CountAsync();

            return existingCount == skillIds.Distinct().Count();
        }

        public async Task UpdateProfileAsync(Freelancer freelancer, List<FreelancerSkill> newSkills)
        {
            var existingSkills = await _context.Set<FreelancerSkill>()
                .Where(fs => fs.FreelancerId == freelancer.UserId)
                .ToListAsync();

            _context.Set<FreelancerSkill>().RemoveRange(existingSkills);
            _context.Freelancers.Update(freelancer);
            await _context.Set<FreelancerSkill>().AddRangeAsync(newSkills);

            await _context.SaveChangesAsync();
        }
    }
}
