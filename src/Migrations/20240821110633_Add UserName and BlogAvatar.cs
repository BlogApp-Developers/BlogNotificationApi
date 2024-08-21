using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogNotificationApi.Migrations
{
    /// <inheritdoc />
    public partial class AddUserNameandBlogAvatar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BlogAvatar",
                table: "Notifications",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Notifications",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlogAvatar",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Notifications");
        }
    }
}
