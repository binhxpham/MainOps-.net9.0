using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class subprojectonsummary : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SubProjectId",
                table: "SummaryReports",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SummaryReports_SubProjectId",
                table: "SummaryReports",
                column: "SubProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_SummaryReports_SubProjects_SubProjectId",
                table: "SummaryReports",
                column: "SubProjectId",
                principalTable: "SubProjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SummaryReports_SubProjects_SubProjectId",
                table: "SummaryReports");

            migrationBuilder.DropIndex(
                name: "IX_SummaryReports_SubProjectId",
                table: "SummaryReports");

            migrationBuilder.DropColumn(
                name: "SubProjectId",
                table: "SummaryReports");
        }
    }
}
