using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class noacidwm : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Acid_Meter_After",
                table: "AcidTreatments");

            migrationBuilder.DropColumn(
                name: "Acid_Meter_Before",
                table: "AcidTreatments");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Acid_Meter_After",
                table: "AcidTreatments",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Acid_Meter_Before",
                table: "AcidTreatments",
                nullable: true);
        }
    }
}
