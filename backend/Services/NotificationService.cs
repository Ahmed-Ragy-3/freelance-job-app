using backend.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using backend.NotificationBuilders;

namespace backend.Services;


[Authorize]
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

    public async Task SendNotificationAsync(INotificationBuilder notificationBuilder) {
        Notification notification = notificationBuilder.BuildNotification();

        appDbContext.Notifications.Add(notification);
        await appDbContext.SaveChangesAsync();

        // Send via SignalR
        if (notification != null) {
            await hubContext.Clients.User(notification.UserId.ToString())
                .SendAsync("ReceiveNotification", notification);
        }
    }
}