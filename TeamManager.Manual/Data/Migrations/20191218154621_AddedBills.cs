using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TeamManager.Manual.Data.Migrations
{
    public partial class AddedBills : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bills",
                schema: "TeamManager",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(nullable: false),
                    Amount = table.Column<int>(nullable: false),
                    PurchaseDate = table.Column<DateTime>(nullable: false),
                    Url = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bills_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "TeamManager",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bills_UserId",
                schema: "TeamManager",
                table: "Bills",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bills",
                schema: "TeamManager");
        }
    }
}
