using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class removingidfromtable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_ProjectStatusProjectCategories_Id",
                table: "ProjectStatusProjectCategories");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ProjectStatusProjectCategories");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ProjectStatusProjectCategories",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_ProjectStatusProjectCategories_Id",
                table: "ProjectStatusProjectCategories",
                column: "Id");
        }
    }
}
