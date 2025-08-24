using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class nodocumentswtpblock : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_WTP_blocks_WTP_blockid",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Documents_WTP_blockid",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "WTP_blockid",
                table: "Documents");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WTP_blockid",
                table: "Documents",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_WTP_blockid",
                table: "Documents",
                column: "WTP_blockid");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_WTP_blocks_WTP_blockid",
                table: "Documents",
                column: "WTP_blockid",
                principalTable: "WTP_blocks",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
