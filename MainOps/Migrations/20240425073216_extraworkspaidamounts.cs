using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class extraworkspaidamounts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "PaidAmount",
                table: "ExtraWorks",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "PaidAmountRental",
                table: "ExtraWorks",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaidAmount",
                table: "ExtraWorks");

            migrationBuilder.DropColumn(
                name: "PaidAmountRental",
                table: "ExtraWorks");
        }
    }
}
