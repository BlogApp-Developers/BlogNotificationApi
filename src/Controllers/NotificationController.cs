using Microsoft.AspNetCore.Mvc;

namespace BlogNotificationApi.Controllers;

using BlogNotificationApi.Data;
using Microsoft.AspNetCore.Components;
using BlogNotificationApi.Notification.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

[Route("api/[controller]")]
[ApiController]
public class NotificationsController : ControllerBase
{
    private readonly NotificationsDbContext dbContext;

    public NotificationsController(NotificationsDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    [Authorize]
    [HttpGet("{userId}")]
    public async Task<ActionResult<IEnumerable<Notification>>> GetUserNotifications(Guid userId)
    {
        return await this.dbContext.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<Notification>> CreateNotification(Notification notification)
    {
        this.dbContext.Notifications.Add(notification);
        await this.dbContext.SaveChangesAsync();
        return CreatedAtAction(nameof(GetUserNotifications), new { userId = notification.UserId }, notification);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        var notification = await this.dbContext.Notifications.FindAsync(id);
        if (notification == null) 
            return NotFound();

        notification.IsRead = true;
        await this.dbContext.SaveChangesAsync();

        return NoContent();
    }
}
