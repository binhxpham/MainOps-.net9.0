using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class divisiononpumptestings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DivisionId",
                table: "PumpTestings",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PumpTestings_DivisionId",
                table: "PumpTestings",
                column: "DivisionId");

            migrationBuilder.AddForeignKey(
                name: "FK_PumpTestings_Divisions_DivisionId",
                table: "PumpTestings",
                column: "DivisionId",
                principalTable: "Divisions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PumpTestings_Divisions_DivisionId",
                table: "PumpTestings");

            migrationBuilder.DropIndex(
                name: "IX_PumpTestings_DivisionId",
                table: "PumpTestings");

            migrationBuilder.DropColumn(
                name: "DivisionId",
                table: "PumpTestings");
        }
    }
}
