using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class amountcontainersindrillwater : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DrillEnd",
                table: "WaterHandlings",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DrillStart",
                table: "WaterHandlings",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "PumpDateEnd",
                table: "WaterHandlings",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "PumpDateStart",
                table: "WaterHandlings",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "AmountContainers",
                table: "DrillWaters",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DrillEnd",
                table: "WaterHandlings");

            migrationBuilder.DropColumn(
                name: "DrillStart",
                table: "WaterHandlings");

            migrationBuilder.DropColumn(
                name: "PumpDateEnd",
                table: "WaterHandlings");

            migrationBuilder.DropColumn(
                name: "PumpDateStart",
                table: "WaterHandlings");

            migrationBuilder.DropColumn(
                name: "AmountContainers",
                table: "DrillWaters");
        }
    }
}
