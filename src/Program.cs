using BlogNotificationApi.Data;
using BlogNotificationApi.Extensions.ServiceCollectionExtensions;
using BlogNotificationApi.Hubs;
using BlogNotificationApi.Methods;
using BlogNotificationApi.User.Repositories;
using BlogNotificationApi.User.Repositories.Base;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSignalR();
builder.Services.AddTransient<TokenValidation>();
builder.Services.AddControllers();
builder.Services.InitSwagger();
builder.Services.InitCors();
builder.Services.InitAuth(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddTransient<IUserRepository, UserDapperRepository>();

builder.Services.AddDbContext<NotificationsDbContext>(options =>
    {var connectionString = builder.Configuration.GetConnectionString("PostgreSqlDev");
            options.UseNpgsql(connectionString);});

var app = builder.Build();

app.MapControllers();
app.MapHub<NotificationsHub>("/NotificationsHub");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("BlazorTestPolicy");

app.Run();