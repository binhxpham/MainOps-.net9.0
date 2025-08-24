using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class snapshotdchanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SnapShotItems_ItemTypes_ItemTypeId",
                table: "SnapShotItems");

            migrationBuilder.AlterColumn<int>(
                name: "ItemTypeId",
                table: "SnapShotItems",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_SnapShotItems_ItemTypes_ItemTypeId",
                table: "SnapShotItems",
                column: "ItemTypeId",
                principalTable: "ItemTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SnapShotItems_ItemTypes_ItemTypeId",
                table: "SnapShotItems");

            migrationBuilder.AlterColumn<int>(
                name: "ItemTypeId",
                table: "SnapShotItems",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SnapShotItems_ItemTypes_ItemTypeId",
                table: "SnapShotItems",
                column: "ItemTypeId",
                principalTable: "ItemTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
