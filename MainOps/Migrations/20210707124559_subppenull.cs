using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class subppenull : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "Sub_PPE",
                table: "ConstructionSiteInspections",
                nullable: true,
                oldClrType: typeof(bool));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "Sub_PPE",
                table: "ConstructionSiteInspections",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);
        }
    }
}
