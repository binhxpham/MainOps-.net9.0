using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class morealarminfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClientAlertPhone",
                table: "Projects",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientContactAlert",
                table: "Projects",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientAlertPhone",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ClientContactAlert",
                table: "Projects");
        }
    }
}
