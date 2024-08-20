namespace BlogNotificationApi.User.Repositories;

using BlogNotificationApi.User.Models;
using BlogNotificationApi.User.Repositories.Base;
using Dapper;
using Npgsql;

public class UserDapperRepository : IUserRepository
{
    private readonly string? connectionString;
    public UserDapperRepository(IConfiguration configuration)
    {
        this.connectionString = configuration.GetConnectionString("PostgreSqlIdentity");
    }

    public async Task<User> GetByIdAsync(Guid id)
    {
        using var connection = new NpgsqlConnection(this.connectionString);

        var user = await connection.QueryFirstAsync<User>("Select \"Email\" From public.\"AspNetUsers\" WHERE public.\"AspNetUsers\".\"Id\" = @Id", new { Id = id });

        return user;
    }

    public async Task UpdateAsync(string email, bool toSend)
    {

        using var connectionIdentity = new NpgsqlConnection(this.connectionString);
        await connectionIdentity.ExecuteAsync($@"Update public.""AspNetUsers""
                                         Set ""SendEmail"" = @SendEmail
                                         Where public.""AspNetUsers"".""Email"" = @Email", new { SendEmail = toSend, Email = email });
    }
}