using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GondolinWeb.Migrations
{
    public partial class AddedProjects : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProjects_AspNetUsers_ApplicationUserId",
                table: "UserProjects");

            migrationBuilder.DropIndex(
                name: "IX_UserProjects_ApplicationUserId",
                table: "UserProjects");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "UserProjects");
        }
    }
}