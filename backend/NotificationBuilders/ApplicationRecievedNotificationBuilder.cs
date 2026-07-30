using backend.Model;

namespace backend.NotificationBuilders;

public class ApplicationReceivedNotificationBuilder(
    int clientId,
    string freelancerName,
    string jobTitle) : INotificationBuilder {
    
    public Notification BuildNotification() {
        return new Notification {
            UserId = clientId,
            Title = "New Application Received",
            Message = $"{freelancerName} submitted an application for your job '{jobTitle}'.",
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };
    }
}