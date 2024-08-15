namespace BlogNotificationApi.Notification.Configurations;

using BlogNotificationApi.Notification.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.UserId).IsRequired();
        builder.Property(u => u.Message).HasMaxLength(256).IsRequired();
        builder.Property(e => e.Type).HasMaxLength(30);
    }
}
