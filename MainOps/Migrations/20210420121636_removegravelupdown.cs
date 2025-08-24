using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class removegravelupdown : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Filter1_Grus_Ned",
                table: "WellDrillingInstructions");

            migrationBuilder.DropColumn(
                name: "Filter1_Grus_Op",
                table: "WellDrillingInstructions");

            migrationBuilder.DropColumn(
                name: "Filter2_Grus_Ned",
                table: "WellDrillingInstructions");

            migrationBuilder.DropColumn(
                name: "Filter2_Grus_Op",
                table: "WellDrillingInstructions");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Filter1_Grus_Ned",
                table: "WellDrillingInstructions",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Filter1_Grus_Op",
                table: "WellDrillingInstructions",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Filter2_Grus_Ned",
                table: "WellDrillingInstructions",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Filter2_Grus_Op",
                table: "WellDrillingInstructions",
                nullable: true);
        }
    }
}
