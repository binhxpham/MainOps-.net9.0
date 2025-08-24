using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class addresses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "SubProjects",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Projects",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "SubProjects");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Projects");
        }
    }
}
