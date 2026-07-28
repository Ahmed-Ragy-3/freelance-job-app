using backend.DTOs;

namespace backend.Repositories
{
    public interface IFreelancerDashboardRepository
    {
        Task<int> GetActiveApplicationsCountAsync(int freelancerId);
        Task<int> GetActiveJobsCountAsync(int freelancerId);
        Task<int> GetCompletedJobsCountAsync(int freelancerId);
        Task<decimal> GetTotalEarningsAsync(int freelancerId);
        Task<int> GetTotalBookmarksCountAsync(int freelancerId);
        Task<int> GetUnreadNotificationsCountAsync(int userId);
        Task<decimal> GetAverageRatingAsync(int freelancerId);
        Task<List<RecentApplicationDto>> GetRecentApplicationsAsync(int freelancerId, int count = 5);
    }
}
