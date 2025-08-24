using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class hephep : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SnapShotItems_Installations_InstallId",
                table: "SnapShotItems");

            migrationBuilder.DropIndex(
                name: "IX_SnapShotItems_InstallId",
                table: "SnapShotItems");

            migrationBuilder.DropColumn(
                name: "InstallId",
                table: "SnapShotItems");

            migrationBuilder.CreateIndex(
                name: "IX_SnapShotItems_InstallationId",
                table: "SnapShotItems",
                column: "InstallationId");

            migrationBuilder.AddForeignKey(
                name: "FK_SnapShotItems_Installations_InstallationId",
                table: "SnapShotItems",
                column: "InstallationId",
                principalTable: "Installations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SnapShotItems_Installations_InstallationId",
                table: "SnapShotItems");

            migrationBuilder.DropIndex(
                name: "IX_SnapShotItems_InstallationId",
                table: "SnapShotItems");

            migrationBuilder.AddColumn<int>(
                name: "InstallId",
                table: "SnapShotItems",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SnapShotItems_InstallId",
                table: "SnapShotItems",
                column: "InstallId");

            migrationBuilder.AddForeignKey(
                name: "FK_SnapShotItems_Installations_InstallId",
                table: "SnapShotItems",
                column: "InstallId",
                principalTable: "Installations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
