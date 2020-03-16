using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TeamManager.Manual.Data.Migrations
{
    public partial class AddedPointConsuptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEntryPaid",
                schema: "TeamManager",
                table: "UserRaces");

            migrationBuilder.DropColumn(
                name: "IsPointsDisabled",
                schema: "TeamManager",
                table: "UserRaces");

            migrationBuilder.DropColumn(
                name: "ConsumedPoints",
                schema: "TeamManager",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "PointConsuptions",
                schema: "TeamManager",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(nullable: false),
                    Amount = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    Remark = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointConsuptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PointConsuptions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "TeamManager",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PointConsuptions_UserId",
                schema: "TeamManager",
                table: "PointConsuptions",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PointConsuptions",
                schema: "TeamManager");

            migrationBuilder.AddColumn<bool>(
                name: "IsEntryPaid",
                schema: "TeamManager",
                table: "UserRaces",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPointsDisabled",
                schema: "TeamManager",
                table: "UserRaces",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ConsumedPoints",
                schema: "TeamManager",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
