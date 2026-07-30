using backend.Model;

namespace backend.NotificationBuilders;

public class ApplicationRejectedNotificationBuilder(
    int freelancerId,
    string jobTitle) : INotificationBuilder {
    
    public Notification BuildNotification() {
        return new Notification {
            UserId = freelancerId,
            Title = "Application Rejected",
            Message = $"Your application for '{jobTitle}' was not selected by the client.",
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };
    }
}