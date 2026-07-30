using backend.Model;

namespace backend.NotificationBuilders;

public class JobEditedNotificationBuilder(
    int clientId,
    string jobTitle) : INotificationBuilder {
    
    public Notification BuildNotification() {
        return new Notification {
            UserId = clientId,
            Title = "Job Updated",
            Message = $"Your job '{jobTitle}' has been updated. Please review the latest changes.",
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };
    }
}