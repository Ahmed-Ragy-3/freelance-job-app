using backend.Model;

namespace backend.NotificationBuilders;

public class ReviewReceivedNotificationBuilder(
    int recipientUserId,
    string reviewerName,
    string jobTitle) : INotificationBuilder {
    
    public Notification BuildNotification() {
        return new Notification {
            UserId = recipientUserId,
            Title = "New Review Received",
            Message = $"{reviewerName} left you a review for the job '{jobTitle}'.",
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };
    }
}