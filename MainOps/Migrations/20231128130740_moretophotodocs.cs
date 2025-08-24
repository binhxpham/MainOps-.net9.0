using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class moretophotodocs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Accuracy",
                table: "PhotoDocumenations",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "PhotoDocumenations",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "PhotoDocumenations",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Accuracy",
                table: "PhotoDocumenations");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "PhotoDocumenations");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "PhotoDocumenations");
        }
    }
}
