using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class newdrillcalcs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "FilterEnd",
                table: "Drillings",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "FilterStart",
                table: "Drillings",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilterEnd",
                table: "Drillings");

            migrationBuilder.DropColumn(
                name: "FilterStart",
                table: "Drillings");
        }
    }
}
