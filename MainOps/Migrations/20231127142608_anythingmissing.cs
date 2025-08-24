using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class anythingmissing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Invoice_date",
                table: "SnapShotItems",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Invoice_date",
                table: "SnapShotItems");
        }
    }
}
