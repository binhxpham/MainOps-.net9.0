using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class boqextraworkmore : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ItemTypeId",
                table: "ExtraWorkBoQItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Number",
                table: "ExtraWorkBoQHeaders",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExtraWorkBoQItems_ItemTypeId",
                table: "ExtraWorkBoQItems",
                column: "ItemTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExtraWorkBoQItems_ItemTypes_ItemTypeId",
                table: "ExtraWorkBoQItems",
                column: "ItemTypeId",
                principalTable: "ItemTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExtraWorkBoQItems_ItemTypes_ItemTypeId",
                table: "ExtraWorkBoQItems");

            migrationBuilder.DropIndex(
                name: "IX_ExtraWorkBoQItems_ItemTypeId",
                table: "ExtraWorkBoQItems");

            migrationBuilder.DropColumn(
                name: "ItemTypeId",
                table: "ExtraWorkBoQItems");

            migrationBuilder.DropColumn(
                name: "Number",
                table: "ExtraWorkBoQHeaders");
        }
    }
}
