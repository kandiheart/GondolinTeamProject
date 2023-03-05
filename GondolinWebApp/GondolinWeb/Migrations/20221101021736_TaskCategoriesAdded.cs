using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GondolinWeb.Migrations
{
    public partial class TaskCategoriesAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "Tasks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsQuickTask",
                table: "Tasks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Project",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Project_ApplicationUserId",
                table: "Project",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Project_AspNetUsers_ApplicationUserId",
                table: "Project",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Project_AspNetUsers_ApplicationUserId",
                table: "Project");

            migrationBuilder.DropIndex(
                name: "IX_Project_ApplicationUserId",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "IsQuickTask",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Project");
        }
    }
}