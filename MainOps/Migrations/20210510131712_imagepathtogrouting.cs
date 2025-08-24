using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class imagepathtogrouting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "imagepath",
                table: "Groutings",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "imagepath",
                table: "Groutings");
        }
    }
}
