using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class personalfiles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Path_DrillProfile",
                table: "Wells");

            migrationBuilder.DropColumn(
                name: "path_DrillLog",
                table: "Wells");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Path_DrillProfile",
                table: "Wells",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "path_DrillLog",
                table: "Wells",
                nullable: true);
        }
    }
}
