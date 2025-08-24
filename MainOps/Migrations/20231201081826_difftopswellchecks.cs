using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class difftopswellchecks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "DiffTop",
                table: "WellChecks",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "DiffTop2",
                table: "WellChecks",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "DiffTop3",
                table: "WellChecks",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiffTop",
                table: "WellChecks");

            migrationBuilder.DropColumn(
                name: "DiffTop2",
                table: "WellChecks");

            migrationBuilder.DropColumn(
                name: "DiffTop3",
                table: "WellChecks");
        }
    }
}
