using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class externalcoords : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "ExternalCoordx",
                table: "MeasPoints",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "ExternalCoordy",
                table: "MeasPoints",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "ExternalCoordz",
                table: "MeasPoints",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalCoordx",
                table: "MeasPoints");

            migrationBuilder.DropColumn(
                name: "ExternalCoordy",
                table: "MeasPoints");

            migrationBuilder.DropColumn(
                name: "ExternalCoordz",
                table: "MeasPoints");
        }
    }
}
