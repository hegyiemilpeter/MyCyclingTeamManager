using Microsoft.EntityFrameworkCore.Migrations;

namespace TeamManager.Manual.Data.Migrations
{
    public partial class PointsAreStoredInDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Points",
                schema: "TeamManager",
                table: "UserRaces",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Points",
                schema: "TeamManager",
                table: "Bills",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Points",
                schema: "TeamManager",
                table: "UserRaces");

            migrationBuilder.DropColumn(
                name: "Points",
                schema: "TeamManager",
                table: "Bills");
        }
    }
}
