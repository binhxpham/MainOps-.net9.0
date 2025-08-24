using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class divisionidonmachinery : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DivisionId",
                table: "Machinery",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Machinery_DivisionId",
                table: "Machinery",
                column: "DivisionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Machinery_Divisions_DivisionId",
                table: "Machinery",
                column: "DivisionId",
                principalTable: "Divisions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Machinery_Divisions_DivisionId",
                table: "Machinery");

            migrationBuilder.DropIndex(
                name: "IX_Machinery_DivisionId",
                table: "Machinery");

            migrationBuilder.DropColumn(
                name: "DivisionId",
                table: "Machinery");
        }
    }
}
