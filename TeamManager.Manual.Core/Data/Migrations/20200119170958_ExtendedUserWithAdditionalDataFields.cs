using Microsoft.EntityFrameworkCore.Migrations;

namespace TeamManager.Manual.Data.Migrations
{
    public partial class ExtendedUserWithAdditionalDataFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BirthPlace",
                schema: "TeamManager",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasEntryStatement",
                schema: "TeamManager",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasGDPRStatement",
                schema: "TeamManager",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "IDNumber",
                schema: "TeamManager",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPro",
                schema: "TeamManager",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "MothersName",
                schema: "TeamManager",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BirthPlace",
                schema: "TeamManager",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "HasEntryStatement",
                schema: "TeamManager",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "HasGDPRStatement",
                schema: "TeamManager",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IDNumber",
                schema: "TeamManager",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsPro",
                schema: "TeamManager",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "MothersName",
                schema: "TeamManager",
                table: "AspNetUsers");
        }
    }
}
