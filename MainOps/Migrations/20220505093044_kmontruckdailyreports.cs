using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class kmontruckdailyreports : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "KM_end",
                table: "TruckSites",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "KM_start",
                table: "TruckSites",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KM_end",
                table: "TruckSites");

            migrationBuilder.DropColumn(
                name: "KM_start",
                table: "TruckSites");
        }
    }
}
