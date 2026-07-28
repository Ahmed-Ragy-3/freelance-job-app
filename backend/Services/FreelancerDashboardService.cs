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
            // Execute queries concurrently for optimal dashboard loading time
            var activeAppsTask = _dashboardRepository.GetActiveApplicationsCountAsync(userId);
            var activeJobsTask = _dashboardRepository.GetActiveJobsCountAsync(userId);
            var completedJobsTask = _dashboardRepository.GetCompletedJobsCountAsync(userId);
            var earningsTask = _dashboardRepository.GetTotalEarningsAsync(userId);
            var bookmarksTask = _dashboardRepository.GetTotalBookmarksCountAsync(userId);
            var unreadNotificationsTask = _dashboardRepository.GetUnreadNotificationsCountAsync(userId);
            var avgRatingTask = _dashboardRepository.GetAverageRatingAsync(userId);
            var recentAppsTask = _dashboardRepository.GetRecentApplicationsAsync(userId, 5);

            await Task.WhenAll(
                activeAppsTask,
                activeJobsTask,
                completedJobsTask,
                earningsTask,
                bookmarksTask,
                unreadNotificationsTask,
                avgRatingTask,
                recentAppsTask
            );

            return new FreelancerDashboardDto
            {
                ActiveApplicationsCount = await activeAppsTask,
                ActiveJobsCount = await activeJobsTask,
                CompletedJobsCount = await completedJobsTask,
                TotalEarnings = await earningsTask,
                TotalBookmarksCount = await bookmarksTask,
                UnreadNotificationsCount = await unreadNotificationsTask,
                AvgRating = await avgRatingTask,
                RecentApplications = await recentAppsTask
            };
        }
    }
}
