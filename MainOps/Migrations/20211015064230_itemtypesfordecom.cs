using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class itemtypesfordecom : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ItemTypeId",
                table: "Decommissions",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Decommissions_ItemTypeId",
                table: "Decommissions",
                column: "ItemTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Decommissions_ItemTypes_ItemTypeId",
                table: "Decommissions",
                column: "ItemTypeId",
                principalTable: "ItemTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Decommissions_ItemTypes_ItemTypeId",
                table: "Decommissions");

            migrationBuilder.DropIndex(
                name: "IX_Decommissions_ItemTypeId",
                table: "Decommissions");

            migrationBuilder.DropColumn(
                name: "ItemTypeId",
                table: "Decommissions");
        }
    }
}
