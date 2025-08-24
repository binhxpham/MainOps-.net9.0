using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class foreignkeysforairlift : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Airlifts_ProjectId",
                table: "Airlifts",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Airlifts_SubProjectId",
                table: "Airlifts",
                column: "SubProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Airlifts_Projects_ProjectId",
                table: "Airlifts",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Airlifts_SubProjects_SubProjectId",
                table: "Airlifts",
                column: "SubProjectId",
                principalTable: "SubProjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Airlifts_Projects_ProjectId",
                table: "Airlifts");

            migrationBuilder.DropForeignKey(
                name: "FK_Airlifts_SubProjects_SubProjectId",
                table: "Airlifts");

            migrationBuilder.DropIndex(
                name: "IX_Airlifts_ProjectId",
                table: "Airlifts");

            migrationBuilder.DropIndex(
                name: "IX_Airlifts_SubProjectId",
                table: "Airlifts");
        }
    }
}
