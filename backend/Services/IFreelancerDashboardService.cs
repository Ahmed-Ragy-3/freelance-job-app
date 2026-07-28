using backend.DTOs;

namespace backend.Services
{
    public interface IFreelancerDashboardService
    {
        Task<FreelancerDashboardDto> GetDashboardOverviewAsync(int userId);
    }
}
