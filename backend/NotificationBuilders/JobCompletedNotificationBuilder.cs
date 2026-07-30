using backend.Model;

namespace backend.NotificationBuilders;

public class JobCompletedNotificationBuilder(
    int clientId,
    string jobTitle) : INotificationBuilder {
    
    public Notification BuildNotification() {
        return new Notification {
            UserId = clientId,
            Title = "Job Completed",
            Message = $"The freelancer has marked the job '{jobTitle}' as completed. Please review the work and confirm completion.",
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };
    }
}