using backend.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;

namespace backend.Services;


public class NotificationHub : Hub {

}

public class NotificationService(AppDbContext appDbContext, IHubContext<NotificationHub> hubContext) {

    public async Task<IEnumerable<Notification>> GetAllAsync(int userId) {
        return await appDbContext.Notifications
                    .Where(n => n.UserId == userId)
                    .OrderByDescending(n => n.CreatedAt)
                    .ToListAsync();
    }

    public async Task<int> GetUnreadCountAsync(int userId) {
        return await appDbContext.Notifications
            .CountAsync(n => n.UserId == userId && !n.IsRead);
    }

    public async Task<bool> MarkAsReadAsync(int notificationId, int userId) {
        var notification = await appDbContext.Notifications
                                .FirstOrDefaultAsync(n =>
                                    n.Id == notificationId &&
                                    n.UserId == userId);

        if (notification == null)
            return false;

        if (!notification.IsRead) {
            notification.IsRead = true;
            await appDbContext.SaveChangesAsync();
        }

        return true;
    }

    public async Task MarkAllAsReadAsync(int userId) {
        var notifications = await appDbContext.Notifications
                                .Where(n => n.UserId == userId && !n.IsRead)
                                .ToListAsync();

        foreach (var notification in notifications)
            notification.IsRead = true;

        await appDbContext.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(int notificationId, int userId) {
        var notification = await appDbContext.Notifications
                                .FirstOrDefaultAsync(n =>
                                    n.Id == notificationId &&
                                    n.UserId == userId);

        if (notification == null)
            return false;

        appDbContext.Notifications.Remove(notification);

        await appDbContext.SaveChangesAsync();

        return true;
    }

    private async Task<Notification> BuildNotification(int userId, NotificationType type) {
        var notification = new Notification {
            UserId = userId,
            IsRead = false,
        };

        switch (type) {
            case NotificationType.ProposalReceived:
                notification.Title = "New Proposal Received";
                notification.Message = "You have received a new proposal for your job posting.";
                break;
            
            case NotificationType.ProposalAccepted:
                notification.Title = "Proposal Accepted";
                notification.Message = "Your proposal has been accepted by the client.";
                break;
            
            case NotificationType.ProposalRejected:
                notification.Title = "Proposal Rejected";
                notification.Message = "Your proposal has been rejected by the client.";
                break;
            
            case NotificationType.JobAssigned:
                notification.Title = "Job Assigned";
                notification.Message = "You have been assigned to a new job.";
                break;
            
            case NotificationType.JobCompleted:
                notification.Title = "Job Completed";
                notification.Message = "The job you were working on has been marked as completed.";
                break;
            
            case NotificationType.ReviewReceived:
                notification.Title = "New Review Received";
                notification.Message = "You have received a new review for your work.";
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        appDbContext.Notifications.Add(notification);

        await appDbContext.SaveChangesAsync();

        return notification;
    }

    public async Task sendNotification(int userId, NotificationType type) {
        Notification notification = await BuildNotification(userId, type);

        // Send via SignalR
        if (notification != null) {
            await hubContext.Clients.User(userId.ToString())
                .SendAsync("ReceiveNotification", notification);
        }
    }
}