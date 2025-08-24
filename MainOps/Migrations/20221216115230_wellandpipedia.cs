using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class wellandpipedia : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PipeDiameter",
                table: "Wells",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WellDiameter",
                table: "Wells",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PipeDiameter",
                table: "Wells");

            migrationBuilder.DropColumn(
                name: "WellDiameter",
                table: "Wells");
        }
    }
}
