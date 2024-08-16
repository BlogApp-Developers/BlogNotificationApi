using Microsoft.AspNetCore.Mvc;

namespace BlogNotificationApi.Controllers;

using BlogNotificationApi.Data;
using Microsoft.AspNetCore.Components;
using BlogNotificationApi.Notification.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using BlogNotificationApi.Methods;
using Microsoft.Extensions.Primitives;

[Route("api/[controller]")]
[ApiController]
public class NotificationsController : ControllerBase
{
    private readonly NotificationsDbContext dbContext;
    private readonly TokenValidation tokenValidation;

    public NotificationsController(NotificationsDbContext dbContext, TokenValidation tokenValidation)
    {
        this.dbContext = dbContext;
        this.tokenValidation = tokenValidation;
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<IEnumerable<Notification>>> GetUserNotifications(Guid userId)
    {
        try
        {
            base.HttpContext.Request.Headers.TryGetValue("Bearer", out StringValues headerValues);
            var tokenNew = headerValues.FirstOrDefault();
            this.tokenValidation.ValidateToken(tokenNew);
        }
        catch (Exception ex)
        {
            return Unauthorized(ex.Message);
        }

        return await this.dbContext.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Notification>> CreateNotification(Notification notification)
    {
        try
        {
            var tokenNew = base.HttpContext.Request.Headers["Bearer"][0];
            this.tokenValidation.ValidateToken(tokenNew);
        }
        catch (Exception ex)
        {
            return Unauthorized(ex.Message);
        }

        this.dbContext.Notifications.Add(notification);
        await this.dbContext.SaveChangesAsync();
        return CreatedAtAction(nameof(GetUserNotifications), new { userId = notification.UserId }, notification);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        try
        {
            var tokenNew = base.HttpContext.Request.Headers["Bearer"][0];
            this.tokenValidation.ValidateToken(tokenNew);
        }
        catch (Exception ex)
        {
            return Unauthorized(ex.Message);
        }

        var notification = await this.dbContext.Notifications.FindAsync(id);
        if (notification == null) 
            return NotFound();

        notification.IsRead = true;
        await this.dbContext.SaveChangesAsync();

        return NoContent();
    }
}
