using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class misspelledtypeofsand : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TýpeOfSand",
                table: "SandTypes",
                newName: "TypeOfSand");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TypeOfSand",
                table: "SandTypes",
                newName: "TýpeOfSand");
        }
    }
}
