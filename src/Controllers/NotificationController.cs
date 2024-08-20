using Microsoft.AspNetCore.Mvc;

namespace BlogNotificationApi.Controllers;

using BlogNotificationApi.Data;
using Microsoft.AspNetCore.Components;
using BlogNotificationApi.Notification.Models;
using Microsoft.EntityFrameworkCore;
using BlogNotificationApi.Methods;
using Microsoft.Extensions.Primitives;
using BlogNotificationApi.Options;
using Microsoft.Extensions.Options;
using BlogNotificationApi.Services.Base;
using BlogNotificationApi.User.Repositories.Base;

[Route("api/[controller]")]
[ApiController]
public class NotificationController : ControllerBase
{
    private readonly NotificationsDbContext dbContext;
    private readonly TokenValidation tokenValidation;
    private readonly EmailOptions emailOptions;
    private readonly IEmailService emailService;
    private readonly IUserRepository userRepository;

    public NotificationController(NotificationsDbContext dbContext, TokenValidation tokenValidation, IOptionsMonitor<EmailOptions> emailOptions, IEmailService emailService, IUserRepository userRepository)
    {
        this.dbContext = dbContext;
        this.tokenValidation = tokenValidation;
        this.emailOptions = emailOptions.CurrentValue;
        this.emailService = emailService;
        this.userRepository = userRepository;
    }

    [HttpGet("api/[controller]/[action]/{userId}")]
    public async Task<ActionResult<IEnumerable<Notification>>> GetUserNotifications(Guid userId)
    {
        try
        {
            base.HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues headerValues);
            var tokenNew = headerValues.FirstOrDefault().Substring(7);
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

    [HttpPost("api/[controller]/[action]")]
    public async Task<ActionResult<Notification>> CreateNotification(Notification notification)
    {
        try
        {
            base.HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues headerValues);
            var tokenNew = headerValues.FirstOrDefault().Substring(7);
            this.tokenValidation.ValidateToken(tokenNew);
        }
        catch (Exception ex)
        {
            return Unauthorized(ex.Message);
        }


        var user = await this.userRepository.GetByIdAsync(notification.UserId);

        this.dbContext.Notifications.Add(notification);
        await this.dbContext.SaveChangesAsync();

        if (user.SendEmail)
        {
            var message = $"{notification.Message}! You can check your notifications following this link: http://20.218.147.124:5234/Notifications";
            await emailService.SendEmailAsync(user.Email, "New Notification!", message);
        }


        return CreatedAtAction(nameof(GetUserNotifications), new { userId = notification.UserId }, notification);
    }

    [HttpPut("api/[controller]/[action]")]
    public async Task<IActionResult> ChangeEmailSend(bool toSend)
    {
        string email;
        try
        {
            base.HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues headerValues);
            var tokenNew = headerValues.FirstOrDefault().Substring(7);
            email = this.tokenValidation.ValidateToken(tokenNew);
        }
        catch (Exception ex)
        {
            return Unauthorized(ex.Message);
        }

        try
        {
            await this.userRepository.UpdateAsync(email, toSend);
        }
        catch (Exception ex)
        {
            return base.BadRequest(ex.Message);
        }

        return base.NoContent();
    }

    [HttpDelete("api/[controller]/[action]")]
    public async Task<IActionResult> DeleteNotification(Guid id)
    {
        try
        {
            base.HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues headerValues);
            var tokenNew = headerValues.FirstOrDefault().Substring(7);
            this.tokenValidation.ValidateToken(tokenNew);
        }
        catch (Exception ex)
        {
            return base.Unauthorized(ex.Message);
        }

        var notification = this.dbContext.Notifications.FirstOrDefault(n => n.Id == id);

        if (notification == null)
        {
            return base.NotFound();
        }

        this.dbContext.Notifications.Remove(notification);
        await this.dbContext.SaveChangesAsync();
        return base.StatusCode(204);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        try
        {
            base.HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues headerValues);
            var tokenNew = headerValues.FirstOrDefault().Substring(7);
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
