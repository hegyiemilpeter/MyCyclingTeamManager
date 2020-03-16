using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TeamManager.Manual.Data.Migrations
{
    public partial class AddedUserRaces : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserRaces",
                schema: "TeamManager",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: false),
                    RaceId = table.Column<int>(nullable: false),
                    IsEntryRequired = table.Column<bool>(nullable: true),
                    IsEntryPaid = table.Column<bool>(nullable: true),
                    IsPointsDisabled = table.Column<bool>(nullable: true),
                    IsTakePartAsStaff = table.Column<bool>(nullable: true),
                    CategoryResult = table.Column<int>(nullable: true),
                    AbsoluteResult = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRaces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRaces_Races_RaceId",
                        column: x => x.RaceId,
                        principalSchema: "TeamManager",
                        principalTable: "Races",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRaces_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "TeamManager",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserRaces_RaceId",
                schema: "TeamManager",
                table: "UserRaces",
                column: "RaceId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRaces_UserId",
                schema: "TeamManager",
                table: "UserRaces",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserRaces",
                schema: "TeamManager");
        }
    }
}
