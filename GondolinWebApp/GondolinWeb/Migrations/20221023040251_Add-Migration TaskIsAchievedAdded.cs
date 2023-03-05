using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GondolinWeb.Migrations
{
    public partial class AddMigrationTaskIsAchievedAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsArchieved",
                table: "Tasks",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsArchieved",
                table: "Tasks");
        }
    }
}