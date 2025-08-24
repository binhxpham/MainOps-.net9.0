using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class moretowelldrilling : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "WellDrillingInstructions",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "WellDrillingInstructions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DrillPlace",
                table: "WellDrillingInstructions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WellID",
                table: "WellDrillingInstructions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "WellDrillingInstructions");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "WellDrillingInstructions");

            migrationBuilder.DropColumn(
                name: "DrillPlace",
                table: "WellDrillingInstructions");

            migrationBuilder.DropColumn(
                name: "WellID",
                table: "WellDrillingInstructions");
        }
    }
}
