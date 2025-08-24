using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class moremonitoringsteptest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Moni3LevelData",
                table: "PumpTestDatasDevice",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Moni4LevelData",
                table: "PumpTestDatasDevice",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Moni3LevelData",
                table: "PumpTestDatasDevice");

            migrationBuilder.DropColumn(
                name: "Moni4LevelData",
                table: "PumpTestDatasDevice");
        }
    }
}
