using Microsoft.EntityFrameworkCore.Migrations;

namespace TeamManager.Manual.Data.Migrations
{
    public partial class AddedImageToUserRaces : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ImageIsValid",
                schema: "TeamManager",
                table: "UserRaces",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                schema: "TeamManager",
                table: "UserRaces",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageIsValid",
                schema: "TeamManager",
                table: "UserRaces");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                schema: "TeamManager",
                table: "UserRaces");
        }
    }
}
