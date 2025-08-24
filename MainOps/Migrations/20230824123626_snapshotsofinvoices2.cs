using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class snapshotsofinvoices2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<bool>(
                name: "FullPeriod",
                table: "SnapShotItems",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullPeriod",
                table: "SnapShotItems");

            migrationBuilder.AddColumn<int>(
                name: "InvoiceItemDBId",
                table: "Discounts",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InvoiceItemDBId",
                table: "Discount_Installations",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Discounts_InvoiceItemDBId",
                table: "Discounts",
                column: "InvoiceItemDBId");

            migrationBuilder.CreateIndex(
                name: "IX_Discount_Installations_InvoiceItemDBId",
                table: "Discount_Installations",
                column: "InvoiceItemDBId");

            migrationBuilder.AddForeignKey(
                name: "FK_Discount_Installations_SnapShotItems_InvoiceItemDBId",
                table: "Discount_Installations",
                column: "InvoiceItemDBId",
                principalTable: "SnapShotItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Discounts_SnapShotItems_InvoiceItemDBId",
                table: "Discounts",
                column: "InvoiceItemDBId",
                principalTable: "SnapShotItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
