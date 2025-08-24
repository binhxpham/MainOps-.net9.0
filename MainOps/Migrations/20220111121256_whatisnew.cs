using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class whatisnew : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HourSheetYear",
                table: "HourRegistrations_Ongoing",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HourSheetYear",
                table: "HourRegistrations",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HourSheetYear",
                table: "HourRegistrations_Ongoing");

            migrationBuilder.DropColumn(
                name: "HourSheetYear",
                table: "HourRegistrations");
        }
    }
}
