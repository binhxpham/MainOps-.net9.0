using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class externallinksondocuments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DivisionId",
                table: "Documents",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalUrl",
                table: "Documents",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_DivisionId",
                table: "Documents",
                column: "DivisionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Divisions_DivisionId",
                table: "Documents",
                column: "DivisionId",
                principalTable: "Divisions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Divisions_DivisionId",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Documents_DivisionId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "DivisionId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "ExternalUrl",
                table: "Documents");
        }
    }
}
