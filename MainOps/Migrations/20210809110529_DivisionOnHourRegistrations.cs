using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class DivisionOnHourRegistrations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HourRegistrations_Divisions_DivisionId",
                table: "HourRegistrations");

            migrationBuilder.DropForeignKey(
                name: "FK_HourRegistrations_Ongoing_Divisions_DivisionId",
                table: "HourRegistrations_Ongoing");

            migrationBuilder.DropIndex(
                name: "IX_HourRegistrations_Ongoing_DivisionId",
                table: "HourRegistrations_Ongoing");

            migrationBuilder.DropIndex(
                name: "IX_HourRegistrations_DivisionId",
                table: "HourRegistrations");

            migrationBuilder.DropColumn(
                name: "DivisionId",
                table: "HourRegistrations_Ongoing");

            migrationBuilder.DropColumn(
                name: "DivisionId",
                table: "HourRegistrations");
        }
    }
}
