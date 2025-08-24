using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class rentalinmainopsnow : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StockRentalItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    HJItemId = table.Column<int>(nullable: true),
                    ItemNumber = table.Column<string>(nullable: true),
                    Company = table.Column<string>(nullable: true),
                    ContactPerson = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    PhoneNr = table.Column<string>(nullable: true),
                    StartTime = table.Column<DateTime>(nullable: false),
                    EndTime = table.Column<DateTime>(nullable: true),
                    TimeSetup = table.Column<DateTime>(nullable: true),
                    RentalFee = table.Column<decimal>(nullable: false),
                    Rental_UnitId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockRentalItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockRentalItems_HJItems_HJItemId",
                        column: x => x.HJItemId,
                        principalTable: "HJItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockRentalItems_Units_Rental_UnitId",
                        column: x => x.Rental_UnitId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StockRentalItemPhotosDelivery",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    StockRentalItemId = table.Column<int>(nullable: true),
                    path = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockRentalItemPhotosDelivery", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockRentalItemPhotosDelivery_StockRentalItems_StockRentalIt~",
                        column: x => x.StockRentalItemId,
                        principalTable: "StockRentalItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StockRentalItemPhotosReturn",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    StockRentalItemId = table.Column<int>(nullable: true),
                    path = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockRentalItemPhotosReturn", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockRentalItemPhotosReturn_StockRentalItems_StockRentalItem~",
                        column: x => x.StockRentalItemId,
                        principalTable: "StockRentalItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StockRentalItemPhotosDelivery_StockRentalItemId",
                table: "StockRentalItemPhotosDelivery",
                column: "StockRentalItemId");

            migrationBuilder.CreateIndex(
                name: "IX_StockRentalItemPhotosReturn_StockRentalItemId",
                table: "StockRentalItemPhotosReturn",
                column: "StockRentalItemId");

            migrationBuilder.CreateIndex(
                name: "IX_StockRentalItems_HJItemId",
                table: "StockRentalItems",
                column: "HJItemId");

            migrationBuilder.CreateIndex(
                name: "IX_StockRentalItems_Rental_UnitId",
                table: "StockRentalItems",
                column: "Rental_UnitId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StockRentalItemPhotosDelivery");

            migrationBuilder.DropTable(
                name: "StockRentalItemPhotosReturn");

            migrationBuilder.DropTable(
                name: "StockRentalItems");
        }
    }
}
