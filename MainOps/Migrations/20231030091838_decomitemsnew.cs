using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class decomitemsnew : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DecommissionableItems_ItemTypes_ItemTypeId",
                table: "DecommissionableItems");

            migrationBuilder.DropForeignKey(
                name: "FK_DecommissionableItems_Projects_ProjectId",
                table: "DecommissionableItems");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "DecommissionableItems");

            migrationBuilder.RenameColumn(
                name: "ProjectId",
                table: "DecommissionableItems",
                newName: "InstalledItemTypeId");

            migrationBuilder.RenameColumn(
                name: "ItemTypeId",
                table: "DecommissionableItems",
                newName: "BoQItemTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_DecommissionableItems_ProjectId",
                table: "DecommissionableItems",
                newName: "IX_DecommissionableItems_InstalledItemTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_DecommissionableItems_ItemTypeId",
                table: "DecommissionableItems",
                newName: "IX_DecommissionableItems_BoQItemTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_DecommissionableItems_ItemTypes_BoQItemTypeId",
                table: "DecommissionableItems",
                column: "BoQItemTypeId",
                principalTable: "ItemTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DecommissionableItems_ItemTypes_InstalledItemTypeId",
                table: "DecommissionableItems",
                column: "InstalledItemTypeId",
                principalTable: "ItemTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DecommissionableItems_ItemTypes_BoQItemTypeId",
                table: "DecommissionableItems");

            migrationBuilder.DropForeignKey(
                name: "FK_DecommissionableItems_ItemTypes_InstalledItemTypeId",
                table: "DecommissionableItems");

            migrationBuilder.RenameColumn(
                name: "InstalledItemTypeId",
                table: "DecommissionableItems",
                newName: "ProjectId");

            migrationBuilder.RenameColumn(
                name: "BoQItemTypeId",
                table: "DecommissionableItems",
                newName: "ItemTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_DecommissionableItems_InstalledItemTypeId",
                table: "DecommissionableItems",
                newName: "IX_DecommissionableItems_ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_DecommissionableItems_BoQItemTypeId",
                table: "DecommissionableItems",
                newName: "IX_DecommissionableItems_ItemTypeId");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "DecommissionableItems",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DecommissionableItems_ItemTypes_ItemTypeId",
                table: "DecommissionableItems",
                column: "ItemTypeId",
                principalTable: "ItemTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DecommissionableItems_Projects_ProjectId",
                table: "DecommissionableItems",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
