using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class arrivalvariationorder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VariationOrderId",
                table: "Arrivals",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Arrivals_VariationOrderId",
                table: "Arrivals",
                column: "VariationOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Arrivals_BoQHeadLines_VariationOrderId",
                table: "Arrivals",
                column: "VariationOrderId",
                principalTable: "BoQHeadLines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Arrivals_BoQHeadLines_VariationOrderId",
                table: "Arrivals");

            migrationBuilder.DropIndex(
                name: "IX_Arrivals_VariationOrderId",
                table: "Arrivals");

            migrationBuilder.DropColumn(
                name: "VariationOrderId",
                table: "Arrivals");
        }
    }
}
