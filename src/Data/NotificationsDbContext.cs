namespace BlogNotificationApi.Data;

using Microsoft.EntityFrameworkCore;
using BlogNotificationApi.Notification.Models;
using BlogNotificationApi.Notification.Configurations;

public class NotificationsDbContext : DbContext
{
    public NotificationsDbContext(DbContextOptions<NotificationsDbContext> options)
        : base(options) {}

    public DbSet<Notification> Notifications { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfiguration(new NotificationConfiguration());
    }
}