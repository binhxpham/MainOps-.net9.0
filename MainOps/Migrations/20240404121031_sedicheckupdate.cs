using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class sedicheckupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Timestamp",
                table: "SedimentationSiteReports",
                newName: "TimeStamp");

            migrationBuilder.AddColumn<string>(
                name: "PlantId",
                table: "SedimentationSiteReports",
                maxLength: 1500,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlantId",
                table: "SedimentationSiteReports");

            migrationBuilder.RenameColumn(
                name: "TimeStamp",
                table: "SedimentationSiteReports",
                newName: "Timestamp");
        }
    }
}
