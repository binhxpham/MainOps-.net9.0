using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class morepipes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "BottomWell2",
                table: "WellChecks",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "BottomWell3",
                table: "WellChecks",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Dip2",
                table: "WellChecks",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Dip3",
                table: "WellChecks",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BottomWell2",
                table: "WellChecks");

            migrationBuilder.DropColumn(
                name: "BottomWell3",
                table: "WellChecks");

            migrationBuilder.DropColumn(
                name: "Dip2",
                table: "WellChecks");

            migrationBuilder.DropColumn(
                name: "Dip3",
                table: "WellChecks");
        }
    }
}
