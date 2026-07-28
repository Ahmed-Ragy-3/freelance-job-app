using backend.Dtos;

namespace backend.Services
{
    public interface IFreelancerService
    {
        Task<FreelancerProfileDto?> GetProfileByUserIdAsync(int userId);
        Task<FreelancerProfileDto?> UpdateProfileAsync(int userId, UpdateFreelancerProfileDto dto);
    }
}
