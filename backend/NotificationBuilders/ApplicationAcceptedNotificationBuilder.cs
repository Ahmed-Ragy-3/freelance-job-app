using backend.Model;

namespace backend.NotificationBuilders;

public class ApplicationAcceptedNotificationBuilder(
    int freelancerId,
    string jobTitle) : INotificationBuilder {
    public Notification BuildNotification() {
        return new Notification {
            UserId = freelancerId,
            Title = "Application Accepted",
            Message = $"Congratulations! Your application for '{jobTitle}' has been accepted by the client.",
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };
    }
}