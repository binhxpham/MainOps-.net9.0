using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class divisiononphonelist : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DivisionId",
                table: "TelefonListen",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TelefonListen_DivisionId",
                table: "TelefonListen",
                column: "DivisionId");

            migrationBuilder.AddForeignKey(
                name: "FK_TelefonListen_Divisions_DivisionId",
                table: "TelefonListen",
                column: "DivisionId",
                principalTable: "Divisions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TelefonListen_Divisions_DivisionId",
                table: "TelefonListen");

            migrationBuilder.DropIndex(
                name: "IX_TelefonListen_DivisionId",
                table: "TelefonListen");

            migrationBuilder.DropColumn(
                name: "DivisionId",
                table: "TelefonListen");
        }
    }
}
