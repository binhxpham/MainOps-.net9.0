using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class updateddivisions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Divisions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrencyDecimalSeperator",
                table: "Divisions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrencyGroupSeperator",
                table: "Divisions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Divisions");

            migrationBuilder.DropColumn(
                name: "CurrencyDecimalSeperator",
                table: "Divisions");

            migrationBuilder.DropColumn(
                name: "CurrencyGroupSeperator",
                table: "Divisions");
        }
    }
}
