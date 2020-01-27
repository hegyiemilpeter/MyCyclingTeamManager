using Microsoft.EntityFrameworkCore.Migrations;

namespace TeamManager.Manual.Data.Migrations
{
    public partial class RemovedDriverAsResult : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsTakePartAsDriver",
                schema: "TeamManager",
                table: "UserRaces");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsTakePartAsDriver",
                schema: "TeamManager",
                table: "UserRaces",
                type: "bit",
                nullable: true);
        }
    }
}
