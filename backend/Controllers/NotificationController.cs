using backend.Auth;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize]
public class NotificationController(NotificationService notificationService) : ControllerBase {
    private int GetUserId() {
        var userId = User.GetUserId();
        if (!userId.HasValue)
            throw new UnauthorizedAccessException("Invalid or missing user identity in JWT token.");
        return userId.Value;
    }

    [HttpGet]
    public async Task<IActionResult> GetNotifications() {
        int userId = GetUserId();
        var notifications = await notificationService.GetAllAsync(userId);

        return Ok(notifications);
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount() {
        int userId = GetUserId();
        var count = await notificationService.GetUnreadCountAsync(userId);

        return Ok(new {
            UnreadCount = count
        });
    }

    [HttpPatch("{id:int}/read")]
    public async Task<IActionResult> MarkAsRead(int id) {
        int userId = GetUserId();
        var success = await notificationService.MarkAsReadAsync(id, userId);

        if (!success)
            return NotFound();

        return NoContent();
    }

    [HttpPatch("read-all")]
    public async Task<IActionResult> MarkAllAsRead() {
        int userId = GetUserId();
        await notificationService.MarkAllAsReadAsync(userId);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) {
        int userId = GetUserId();
        var success = await notificationService.DeleteAsync(id, userId);

        if (!success)
            return NotFound();

        return NoContent();
    }
}