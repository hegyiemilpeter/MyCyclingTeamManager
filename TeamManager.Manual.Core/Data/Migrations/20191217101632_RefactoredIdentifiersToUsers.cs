using Microsoft.EntityFrameworkCore.Migrations;

namespace TeamManager.Manual.Data.Migrations
{
    public partial class RefactoredIdentifiersToUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IdentificationNumbers",
                schema: "TeamManager");

            migrationBuilder.AddColumn<string>(
                name: "AkeszNumber",
                schema: "TeamManager",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtprobaNumber",
                schema: "TeamManager",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TriathleteLicence",
                schema: "TeamManager",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UCILicence",
                schema: "TeamManager",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AkeszNumber",
                schema: "TeamManager",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "OtprobaNumber",
                schema: "TeamManager",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TriathleteLicence",
                schema: "TeamManager",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UCILicence",
                schema: "TeamManager",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "IdentificationNumbers",
                schema: "TeamManager",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentificationNumbers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentificationNumbers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "TeamManager",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IdentificationNumbers_UserId",
                schema: "TeamManager",
                table: "IdentificationNumbers",
                column: "UserId");
        }
    }
}
