using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TeamManager.Manual.Data.Migrations
{
    public partial class EntryDeadlineCanBeNull : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "EntryDeadline",
                schema: "TeamManager",
                table: "Races",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "EntryDeadline",
                schema: "TeamManager",
                table: "Races",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);
        }
    }
}
