using backend.Model;

namespace backend.Repositories
{
    public interface IFreelancerRepository
    {
        Task<Freelancer?> GetByUserIdAsync(int userId);
        Task<List<FreelancerSkill>> GetFreelancerSkillsAsync(int userId);
        Task<decimal> GetAverageRatingAsync(int userId);
        Task<bool> SkillsExistAsync(IEnumerable<int> skillIds);
        Task UpdateProfileAsync(Freelancer freelancer, List<FreelancerSkill> newSkills);
    }
}
