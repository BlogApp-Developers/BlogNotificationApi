namespace BlogNotificationApi.Hubs;

using Microsoft.AspNetCore.SignalR;
using BlogNotificationApi.Notification.Models;

public class NotificationsHub : Hub
{
    public async Task SendNotification(string userId, Notification notification)
    {
        await Clients.User(userId).SendAsync("ReceiveNotification", notification);
    }
}
