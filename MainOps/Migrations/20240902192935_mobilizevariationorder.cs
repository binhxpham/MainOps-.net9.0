using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class mobilizevariationorder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VariationOrderId",
                table: "Mobilisations",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Mobilisations_VariationOrderId",
                table: "Mobilisations",
                column: "VariationOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Mobilisations_BoQHeadLines_VariationOrderId",
                table: "Mobilisations",
                column: "VariationOrderId",
                principalTable: "BoQHeadLines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mobilisations_BoQHeadLines_VariationOrderId",
                table: "Mobilisations");

            migrationBuilder.DropIndex(
                name: "IX_Mobilisations_VariationOrderId",
                table: "Mobilisations");

            migrationBuilder.DropColumn(
                name: "VariationOrderId",
                table: "Mobilisations");
        }
    }
}
