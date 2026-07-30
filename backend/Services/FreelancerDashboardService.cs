using backend.DTOs;
using backend.Repositories;

namespace backend.Services
{
    public class FreelancerDashboardService : IFreelancerDashboardService
    {
        private readonly IFreelancerDashboardRepository _dashboardRepository;

        public FreelancerDashboardService(IFreelancerDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        public async Task<FreelancerDashboardDto> GetDashboardOverviewAsync(int userId)
        {
            // DbContext is not thread-safe — run queries sequentially on the shared scoped instance.
            var activeApps = await _dashboardRepository.GetActiveApplicationsCountAsync(userId);
            var activeJobs = await _dashboardRepository.GetActiveJobsCountAsync(userId);
            var completedJobs = await _dashboardRepository.GetCompletedJobsCountAsync(userId);
            var earnings = await _dashboardRepository.GetTotalEarningsAsync(userId);
            var bookmarks = await _dashboardRepository.GetTotalBookmarksCountAsync(userId);
            var unreadNotifications = await _dashboardRepository.GetUnreadNotificationsCountAsync(userId);
            var avgRating = await _dashboardRepository.GetAverageRatingAsync(userId);
            var recentApps = await _dashboardRepository.GetRecentApplicationsAsync(userId, 5);

            return new FreelancerDashboardDto
            {
                ActiveApplicationsCount = activeApps,
                ActiveJobsCount = activeJobs,
                CompletedJobsCount = completedJobs,
                TotalEarnings = earnings,
                TotalBookmarksCount = bookmarks,
                UnreadNotificationsCount = unreadNotifications,
                AvgRating = avgRating,
                RecentApplications = recentApps
            };
        }
    }
}
