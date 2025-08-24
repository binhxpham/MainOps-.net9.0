using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class divisiononmasterclasses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DivisionId",
                table: "HJItemMasterClasses",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HJItemMasterClasses_DivisionId",
                table: "HJItemMasterClasses",
                column: "DivisionId");

            migrationBuilder.AddForeignKey(
                name: "FK_HJItemMasterClasses_Divisions_DivisionId",
                table: "HJItemMasterClasses",
                column: "DivisionId",
                principalTable: "Divisions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HJItemMasterClasses_Divisions_DivisionId",
                table: "HJItemMasterClasses");

            migrationBuilder.DropIndex(
                name: "IX_HJItemMasterClasses_DivisionId",
                table: "HJItemMasterClasses");

            migrationBuilder.DropColumn(
                name: "DivisionId",
                table: "HJItemMasterClasses");
        }
    }
}
