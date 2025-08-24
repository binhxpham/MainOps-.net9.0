using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class updatedwatersamplemeasures : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateLabReceived",
                table: "WaterSampleMeasures",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateNextSample",
                table: "WaterSampleMeasures",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateReporting",
                table: "WaterSampleMeasures",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SampleTakerName",
                table: "WaterSampleMeasures",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateLabReceived",
                table: "WaterSampleMeasures");

            migrationBuilder.DropColumn(
                name: "DateNextSample",
                table: "WaterSampleMeasures");

            migrationBuilder.DropColumn(
                name: "DateReporting",
                table: "WaterSampleMeasures");

            migrationBuilder.DropColumn(
                name: "SampleTakerName",
                table: "WaterSampleMeasures");
        }
    }
}
