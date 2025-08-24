using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class subprojongrout : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Groutings",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Groutings",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "SubProjectId",
                table: "Groutings",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Groutings_SubProjectId",
                table: "Groutings",
                column: "SubProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Groutings_SubProjects_SubProjectId",
                table: "Groutings",
                column: "SubProjectId",
                principalTable: "SubProjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groutings_SubProjects_SubProjectId",
                table: "Groutings");

            migrationBuilder.DropIndex(
                name: "IX_Groutings_SubProjectId",
                table: "Groutings");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Groutings");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Groutings");

            migrationBuilder.DropColumn(
                name: "SubProjectId",
                table: "Groutings");
        }
    }
}
