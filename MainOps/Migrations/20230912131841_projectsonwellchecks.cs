using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class projectsonwellchecks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "WellChecks",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WellChecks_ProjectId",
                table: "WellChecks",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_WellChecks_Projects_ProjectId",
                table: "WellChecks",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WellChecks_Projects_ProjectId",
                table: "WellChecks");

            migrationBuilder.DropIndex(
                name: "IX_WellChecks_ProjectId",
                table: "WellChecks");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "WellChecks");
        }
    }
}
