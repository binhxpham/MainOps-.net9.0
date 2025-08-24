using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class hmmm2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TheUserId",
                table: "AlarmReportReceivers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AlarmReportReceivers_TheUserId",
                table: "AlarmReportReceivers",
                column: "TheUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AlarmReportReceivers_AspNetUsers_TheUserId",
                table: "AlarmReportReceivers",
                column: "TheUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
