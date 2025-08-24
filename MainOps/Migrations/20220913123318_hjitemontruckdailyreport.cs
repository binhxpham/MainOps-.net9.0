using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class hjitemontruckdailyreport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HJItemId",
                table: "TruckDailyReports",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TruckDailyReports_HJItemId",
                table: "TruckDailyReports",
                column: "HJItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_TruckDailyReports_HJItems_HJItemId",
                table: "TruckDailyReports",
                column: "HJItemId",
                principalTable: "HJItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TruckDailyReports_HJItems_HJItemId",
                table: "TruckDailyReports");

            migrationBuilder.DropIndex(
                name: "IX_TruckDailyReports_HJItemId",
                table: "TruckDailyReports");

            migrationBuilder.DropColumn(
                name: "HJItemId",
                table: "TruckDailyReports");
        }
    }
}
