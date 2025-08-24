using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class preexcavationincrease : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "NewCover",
                table: "PreExcavations",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RemovalOldManShaft",
                table: "PreExcavations",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "VariationOrderId",
                table: "PreExcavations",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PreExcavations_VariationOrderId",
                table: "PreExcavations",
                column: "VariationOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_PreExcavations_BoQHeadLines_VariationOrderId",
                table: "PreExcavations",
                column: "VariationOrderId",
                principalTable: "BoQHeadLines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PreExcavations_BoQHeadLines_VariationOrderId",
                table: "PreExcavations");

            migrationBuilder.DropIndex(
                name: "IX_PreExcavations_VariationOrderId",
                table: "PreExcavations");

            migrationBuilder.DropColumn(
                name: "NewCover",
                table: "PreExcavations");

            migrationBuilder.DropColumn(
                name: "RemovalOldManShaft",
                table: "PreExcavations");

            migrationBuilder.DropColumn(
                name: "VariationOrderId",
                table: "PreExcavations");
        }
    }
}
