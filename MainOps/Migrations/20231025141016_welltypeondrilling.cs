using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class welltypeondrilling : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WellTypeId",
                table: "Wells",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wells_WellTypeId",
                table: "Wells",
                column: "WellTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Wells_WellTypes_WellTypeId",
                table: "Wells",
                column: "WellTypeId",
                principalTable: "WellTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Wells_WellTypes_WellTypeId",
                table: "Wells");

            migrationBuilder.DropIndex(
                name: "IX_Wells_WellTypeId",
                table: "Wells");

            migrationBuilder.DropColumn(
                name: "WellTypeId",
                table: "Wells");
        }
    }
}
