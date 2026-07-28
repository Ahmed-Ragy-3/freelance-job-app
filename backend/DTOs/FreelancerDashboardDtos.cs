using backend.Model;

namespace backend.DTOs
{
    public class RecentApplicationDto
    {
        public int JobId { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string? CompanyLogo { get; set; }
        public int Bid { get; set; }
        public int Timeline { get; set; }
        public AppStatus AppStatus { get; set; }
    }

    public class FreelancerDashboardDto
    {
        public int ActiveApplicationsCount { get; set; }
        public int ActiveJobsCount { get; set; }
        public int CompletedJobsCount { get; set; }
        public decimal TotalEarnings { get; set; }
        public int TotalBookmarksCount { get; set; }
        public decimal AvgRating { get; set; }
        public int UnreadNotificationsCount { get; set; }
        public List<RecentApplicationDto> RecentApplications { get; set; } = new();
    }
}
