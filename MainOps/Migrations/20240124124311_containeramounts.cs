using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class containeramounts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AmountContainerDrill",
                table: "WaterHandlings",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AmountContainerPump",
                table: "WaterHandlings",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountContainerDrill",
                table: "WaterHandlings");

            migrationBuilder.DropColumn(
                name: "AmountContainerPump",
                table: "WaterHandlings");
        }
    }
}
