using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class moretosediments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "OilSeperatorClogged",
                table: "SedimentationSiteReports",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SedimenationFlowRate",
                table: "SedimentationSiteReports",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SedimentationMinimumWater",
                table: "SedimentationSiteReports",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OilSeperatorClogged",
                table: "SedimentationSiteReports");

            migrationBuilder.DropColumn(
                name: "SedimenationFlowRate",
                table: "SedimentationSiteReports");

            migrationBuilder.DropColumn(
                name: "SedimentationMinimumWater",
                table: "SedimentationSiteReports");
        }
    }
}
