using BlogNotificationApi.Data;
using BlogNotificationApi.Hubs;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

builder.Services.AddDbContext<NotificationsDbContext>(options =>
    {var connectionString = builder.Configuration.GetConnectionString("PostgreSqlDev");
            options.UseNpgsql(connectionString);});

var app = builder.Build();

app.MapHub<NotificationsHub>("/NotificationsHub");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();