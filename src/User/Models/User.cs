using Microsoft.AspNetCore.Identity;

namespace BlogNotificationApi.User.Models;

public class User : IdentityUser<Guid>
{
    public string Email { get; set; }
}
