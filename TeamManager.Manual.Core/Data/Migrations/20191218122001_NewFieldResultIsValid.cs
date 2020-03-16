using Microsoft.EntityFrameworkCore.Migrations;

namespace TeamManager.Manual.Data.Migrations
{
    public partial class NewFieldResultIsValid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageIsValid",
                schema: "TeamManager",
                table: "UserRaces");

            migrationBuilder.AddColumn<bool>(
                name: "ResultIsValid",
                schema: "TeamManager",
                table: "UserRaces",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResultIsValid",
                schema: "TeamManager",
                table: "UserRaces");

            migrationBuilder.AddColumn<bool>(
                name: "ImageIsValid",
                schema: "TeamManager",
                table: "UserRaces",
                type: "bit",
                nullable: true);
        }
    }
}
