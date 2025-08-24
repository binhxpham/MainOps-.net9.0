using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class payedamountplusothers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "otherinfo",
                table: "Log2s",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "PayedAmount",
                table: "Arrivals",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "otherinfo",
                table: "Log2s");

            migrationBuilder.DropColumn(
                name: "PayedAmount",
                table: "Arrivals");
        }
    }
}
