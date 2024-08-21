using System.ComponentModel.DataAnnotations;

namespace BlogNotificationApi.Notification.Models;

public class Notification
{
    [Key]
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string? Message { get; set; }
    public string? Type { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? UserName { get; set; }
    public bool IsRead { get; set; } = false;
}