using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GondolinWeb.Migrations
{
    public partial class TasksTable_RollBack : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsArchived",
                table: "Tasks",
                newName: "IsArchieved");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsArchieved",
                table: "Tasks",
                newName: "IsArchived");
        }
    }
}