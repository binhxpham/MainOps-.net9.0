using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class cellstartsandends : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CellEnd",
                table: "Filter2Layers",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CellStart",
                table: "Filter2Layers",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CellEnd",
                table: "Filter1Layers",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CellStart",
                table: "Filter1Layers",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CellEnd",
                table: "BentoniteLayers",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CellStart",
                table: "BentoniteLayers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CellEnd",
                table: "Filter2Layers");

            migrationBuilder.DropColumn(
                name: "CellStart",
                table: "Filter2Layers");

            migrationBuilder.DropColumn(
                name: "CellEnd",
                table: "Filter1Layers");

            migrationBuilder.DropColumn(
                name: "CellStart",
                table: "Filter1Layers");

            migrationBuilder.DropColumn(
                name: "CellEnd",
                table: "BentoniteLayers");

            migrationBuilder.DropColumn(
                name: "CellStart",
                table: "BentoniteLayers");
        }
    }
}
