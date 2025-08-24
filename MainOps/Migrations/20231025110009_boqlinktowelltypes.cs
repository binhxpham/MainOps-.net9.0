using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class boqlinktowelltypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ItemTypeId",
                table: "WellTypes",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WellTypes_ItemTypeId",
                table: "WellTypes",
                column: "ItemTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_WellTypes_ItemTypes_ItemTypeId",
                table: "WellTypes",
                column: "ItemTypeId",
                principalTable: "ItemTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WellTypes_ItemTypes_ItemTypeId",
                table: "WellTypes");

            migrationBuilder.DropIndex(
                name: "IX_WellTypes_ItemTypeId",
                table: "WellTypes");

            migrationBuilder.DropColumn(
                name: "ItemTypeId",
                table: "WellTypes");
        }
    }
}
