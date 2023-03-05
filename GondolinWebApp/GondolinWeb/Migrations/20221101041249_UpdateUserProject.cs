using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GondolinWeb.Migrations
{
    public partial class UpdateUserProject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "UserProjects",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserProjects_ApplicationUserId",
                table: "UserProjects",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProjects_AspNetUsers_ApplicationUserId",
                table: "UserProjects",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
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