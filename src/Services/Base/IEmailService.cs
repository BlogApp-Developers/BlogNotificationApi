namespace BlogNotificationApi.Services.Base;

public interface IEmailService
{
    public Task SendEmailAsync(string toEmail, string subject, string message);
}
