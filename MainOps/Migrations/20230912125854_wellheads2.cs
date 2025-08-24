using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class wellheads2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "WellChecks");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "WellChecks");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "WellCheckPhotos");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "WellCheckPhotos");

            migrationBuilder.AddColumn<bool>(
                name: "WellHeads",
                table: "WellChecks",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WellHeads",
                table: "WellChecks");

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "WellChecks",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "WellChecks",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "WellCheckPhotos",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "WellCheckPhotos",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
