using backend.Model;

namespace backend.NotificationBuilders;

public class JobApprovedNotificationBuilder(
    int clientId,
    string jobTitle) : INotificationBuilder {
    
    public Notification BuildNotification() {
        return new Notification {
            UserId = clientId,
            Title = "Job Approved",
            Message = $"Your job '{jobTitle}' has been approved by an administrator and is now visible to freelancers.",
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };
    }
}