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
        this.connectionString = configuration.GetConnectionString("BlogWebApiDb");
    }

    public async Task<User> GetByIdAsync(Guid id)
    {
        using var connection = new NpgsqlConnection(this.connectionString);

        var user = await connection.QueryFirstAsync<User>("Select \"Email\" From public.\"AspNetUsers\" WHERE u.\"Id\" = @Id", new { Id = id });

        return user;
    }
}