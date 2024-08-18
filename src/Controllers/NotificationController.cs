using Microsoft.AspNetCore.Mvc;

namespace BlogNotificationApi.Controllers;

using BlogNotificationApi.Data;
using Microsoft.AspNetCore.Components;
using BlogNotificationApi.Notification.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using BlogNotificationApi.Methods;
using Microsoft.Extensions.Primitives;
using BlogNotificationApi.Options;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

[Route("api/[controller]")]
[ApiController]
public class NotificationController : ControllerBase
{
    private readonly NotificationsDbContext dbContext;
    private readonly TokenValidation tokenValidation;
    private readonly EmailOptions emailOptions;

    public NotificationController(NotificationsDbContext dbContext, TokenValidation tokenValidation, IOptionsMonitor<EmailOptions> emailOptions)
    {
        this.dbContext = dbContext;
        this.tokenValidation = tokenValidation;
        this.emailOptions = emailOptions.CurrentValue;
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

        this.dbContext.Notifications.Add(notification);
        await this.dbContext.SaveChangesAsync();
        var message = $"{notification.Message}! You can check your notifications following this link: http://localhost:5234/Notifications";
        return CreatedAtAction(nameof(GetUserNotifications), new { userId = notification.UserId }, notification);
    }

    [HttpPut("api/[controller]/[action]")]
    public async Task<IActionResult> ChangeEmailSend(bool value)
    {
        var filePath = Path.Combine("./", "appSettings.Development.json");
        string json = System.IO.File.ReadAllText(filePath);
        IList<JToken> jsonObj = (IList<JToken>)Newtonsoft.Json.JsonConvert.DeserializeObject(json);
        var sectionPath = "ToSend";

        jsonObj.FirstOrDefault(t => t.Path == sectionPath).Remove();
        jsonObj.Last().AddAfterSelf(new JProperty(sectionPath, value));

        string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
        System.IO.File.WriteAllText(filePath, output);

        return base.Ok();
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
