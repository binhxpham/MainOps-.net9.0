using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class startstopwatermeter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "WaterMeterEnd",
                table: "Groutings",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "WaterMeterStart",
                table: "Groutings",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WaterMeterEnd",
                table: "Groutings");

            migrationBuilder.DropColumn(
                name: "WaterMeterStart",
                table: "Groutings");
        }
    }
}
