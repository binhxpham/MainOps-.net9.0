using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class useridonhourregistration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "userid",
                table: "HourRegistrations_Ongoing",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "userid",
                table: "HourRegistrations",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "userid",
                table: "HourRegistrations_Ongoing");

            migrationBuilder.DropColumn(
                name: "userid",
                table: "HourRegistrations");
        }
    }
}
