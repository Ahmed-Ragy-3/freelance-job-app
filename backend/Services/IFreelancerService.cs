using backend.DTOs;

namespace backend.Services
{
    public interface IFreelancerService
    {
        Task<FreelancerProfileDto?> GetProfileByUserIdAsync(int userId);
        Task<FreelancerProfileDto?> UpdateProfileAsync(int userId, UpdateFreelancerProfileDto dto);
        Task<List<FreelancerSummaryDto>> GetTopNFreelancersAsync(int n);

        Task<List<FreelancerSummaryDto>> SearchFreelancersAsync(string searchTerm);
    }
}
