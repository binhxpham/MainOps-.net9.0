using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class snapshotsofinvoices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "SnapShots",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SnapShotStartDate = table.Column<DateTime>(nullable: false),
                    SnapShotEndDate = table.Column<DateTime>(nullable: false),
                    ProjectId = table.Column<int>(nullable: false),
                    SubProjectId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SnapShots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SnapShots_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SnapShots_SubProjects_SubProjectId",
                        column: x => x.SubProjectId,
                        principalTable: "SubProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SnapShotItems",
                columns: table => new
                {
                    Item_Name = table.Column<string>(nullable: true),
                    Amount = table.Column<double>(nullable: false),
                    Days = table.Column<double>(nullable: false),
                    price = table.Column<decimal>(nullable: true),
                    rental_price = table.Column<decimal>(nullable: true),
                    Install_date = table.Column<DateTime>(nullable: false),
                    ItemTypeId = table.Column<int>(nullable: false),
                    MobilizationId = table.Column<int>(nullable: true),
                    MobilizeId = table.Column<int>(nullable: true),
                    InstallationId = table.Column<int>(nullable: true),
                    InstallId = table.Column<int>(nullable: true),
                    ArrivalId = table.Column<int>(nullable: true),
                    ExtraWorkId = table.Column<int>(nullable: true),
                    Daily_Report_2Id = table.Column<int>(nullable: true),
                    BoQNr = table.Column<decimal>(nullable: false),
                    BoQNr_Rental = table.Column<decimal>(nullable: true),
                    location = table.Column<string>(nullable: true),
                    SubProjectId = table.Column<int>(nullable: true),
                    Total_Discount = table.Column<decimal>(nullable: true),
                    Total_Discount_Installation = table.Column<decimal>(nullable: true),
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    InvoiceSnapShotId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SnapShotItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SnapShotItems_Arrivals_ArrivalId",
                        column: x => x.ArrivalId,
                        principalTable: "Arrivals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SnapShotItems_Daily_Report_2s_Daily_Report_2Id",
                        column: x => x.Daily_Report_2Id,
                        principalTable: "Daily_Report_2s",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SnapShotItems_ExtraWorks_ExtraWorkId",
                        column: x => x.ExtraWorkId,
                        principalTable: "ExtraWorks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SnapShotItems_Installations_InstallId",
                        column: x => x.InstallId,
                        principalTable: "Installations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SnapShotItems_SnapShots_InvoiceSnapShotId",
                        column: x => x.InvoiceSnapShotId,
                        principalTable: "SnapShots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SnapShotItems_ItemTypes_ItemTypeId",
                        column: x => x.ItemTypeId,
                        principalTable: "ItemTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SnapShotItems_Mobilisations_MobilizeId",
                        column: x => x.MobilizeId,
                        principalTable: "Mobilisations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });


            migrationBuilder.CreateIndex(
                name: "IX_SnapShotItems_ArrivalId",
                table: "SnapShotItems",
                column: "ArrivalId");

            migrationBuilder.CreateIndex(
                name: "IX_SnapShotItems_Daily_Report_2Id",
                table: "SnapShotItems",
                column: "Daily_Report_2Id");

            migrationBuilder.CreateIndex(
                name: "IX_SnapShotItems_ExtraWorkId",
                table: "SnapShotItems",
                column: "ExtraWorkId");

            migrationBuilder.CreateIndex(
                name: "IX_SnapShotItems_InstallId",
                table: "SnapShotItems",
                column: "InstallId");

            migrationBuilder.CreateIndex(
                name: "IX_SnapShotItems_InvoiceSnapShotId",
                table: "SnapShotItems",
                column: "InvoiceSnapShotId");

            migrationBuilder.CreateIndex(
                name: "IX_SnapShotItems_ItemTypeId",
                table: "SnapShotItems",
                column: "ItemTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SnapShotItems_MobilizeId",
                table: "SnapShotItems",
                column: "MobilizeId");

            migrationBuilder.CreateIndex(
                name: "IX_SnapShots_ProjectId",
                table: "SnapShots",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_SnapShots_SubProjectId",
                table: "SnapShots",
                column: "SubProjectId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Discount_Installations_SnapShotItems_InvoiceItemDBId",
                table: "Discount_Installations");

            migrationBuilder.DropForeignKey(
                name: "FK_Discounts_SnapShotItems_InvoiceItemDBId",
                table: "Discounts");

            migrationBuilder.DropTable(
                name: "SnapShotItems");

            migrationBuilder.DropTable(
                name: "SnapShots");

            migrationBuilder.DropIndex(
                name: "IX_Discounts_InvoiceItemDBId",
                table: "Discounts");

            migrationBuilder.DropIndex(
                name: "IX_Discount_Installations_InvoiceItemDBId",
                table: "Discount_Installations");

            migrationBuilder.DropColumn(
                name: "InvoiceItemDBId",
                table: "Discounts");

            migrationBuilder.DropColumn(
                name: "InvoiceItemDBId",
                table: "Discount_Installations");
        }
    }
}
