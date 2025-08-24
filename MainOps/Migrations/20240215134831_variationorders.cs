using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class variationorders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VariationOrderId",
                table: "Installations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VariationOrderId",
                table: "Daily_Report_2s",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Installations_VariationOrderId",
                table: "Installations",
                column: "VariationOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Daily_Report_2s_VariationOrderId",
                table: "Daily_Report_2s",
                column: "VariationOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Daily_Report_2s_BoQHeadLines_VariationOrderId",
                table: "Daily_Report_2s",
                column: "VariationOrderId",
                principalTable: "BoQHeadLines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Installations_BoQHeadLines_VariationOrderId",
                table: "Installations",
                column: "VariationOrderId",
                principalTable: "BoQHeadLines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Daily_Report_2s_BoQHeadLines_VariationOrderId",
                table: "Daily_Report_2s");

            migrationBuilder.DropForeignKey(
                name: "FK_Installations_BoQHeadLines_VariationOrderId",
                table: "Installations");

            migrationBuilder.DropIndex(
                name: "IX_Installations_VariationOrderId",
                table: "Installations");

            migrationBuilder.DropIndex(
                name: "IX_Daily_Report_2s_VariationOrderId",
                table: "Daily_Report_2s");

            migrationBuilder.DropColumn(
                name: "VariationOrderId",
                table: "Installations");

            migrationBuilder.DropColumn(
                name: "VariationOrderId",
                table: "Daily_Report_2s");
        }
    }
}
