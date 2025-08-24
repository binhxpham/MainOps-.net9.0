using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class ExtraWorkBoQStartup2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExtraWorkBoQs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BoQHeadLineId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExtraWorkBoQs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExtraWorkBoQs_BoQHeadLines_BoQHeadLineId",
                        column: x => x.BoQHeadLineId,
                        principalTable: "BoQHeadLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExtraWorkBoQDescriptions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ExtraWorkBoQId = table.Column<int>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExtraWorkBoQDescriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExtraWorkBoQDescriptions_ExtraWorkBoQs_ExtraWorkBoQId",
                        column: x => x.ExtraWorkBoQId,
                        principalTable: "ExtraWorkBoQs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExtraWorkBoQHeaders",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(nullable: true),
                    ExtraWorkBoQId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExtraWorkBoQHeaders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExtraWorkBoQHeaders_ExtraWorkBoQs_ExtraWorkBoQId",
                        column: x => x.ExtraWorkBoQId,
                        principalTable: "ExtraWorkBoQs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExtraWorkBoQItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    NewBoQNr = table.Column<string>(nullable: true),
                    NewRentalBoQNr = table.Column<string>(nullable: true),
                    BoQItemDescription = table.Column<string>(nullable: true),
                    ExtraWorkBoQHeaderId = table.Column<int>(nullable: true),
                    Item_Type = table.Column<string>(nullable: true),
                    BoQnr = table.Column<decimal>(nullable: false),
                    BoQnr_Rental = table.Column<decimal>(nullable: true),
                    price = table.Column<decimal>(nullable: true),
                    rental_price = table.Column<decimal>(nullable: true),
                    Install_UnitId = table.Column<int>(nullable: true),
                    Rental_UnitId = table.Column<int>(nullable: true),
                    Valuta = table.Column<string>(nullable: true),
                    ReportTypeId = table.Column<int>(nullable: true),
                    MarkerPicture = table.Column<string>(nullable: true),
                    ExpectedAmount = table.Column<double>(nullable: true),
                    ExpectedAmountRental = table.Column<double>(nullable: true),
                    ExtraWorkBoQId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExtraWorkBoQItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExtraWorkBoQItems_ExtraWorkBoQHeaders_ExtraWorkBoQHeaderId",
                        column: x => x.ExtraWorkBoQHeaderId,
                        principalTable: "ExtraWorkBoQHeaders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExtraWorkBoQItems_ExtraWorkBoQs_ExtraWorkBoQId",
                        column: x => x.ExtraWorkBoQId,
                        principalTable: "ExtraWorkBoQs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExtraWorkBoQItems_Units_Install_UnitId",
                        column: x => x.Install_UnitId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExtraWorkBoQItems_Units_Rental_UnitId",
                        column: x => x.Rental_UnitId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExtraWorkBoQItems_ReportTypes_ReportTypeId",
                        column: x => x.ReportTypeId,
                        principalTable: "ReportTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExtraWorkBoQDescriptions_ExtraWorkBoQId",
                table: "ExtraWorkBoQDescriptions",
                column: "ExtraWorkBoQId");

            migrationBuilder.CreateIndex(
                name: "IX_ExtraWorkBoQHeaders_ExtraWorkBoQId",
                table: "ExtraWorkBoQHeaders",
                column: "ExtraWorkBoQId");

            migrationBuilder.CreateIndex(
                name: "IX_ExtraWorkBoQItems_ExtraWorkBoQHeaderId",
                table: "ExtraWorkBoQItems",
                column: "ExtraWorkBoQHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_ExtraWorkBoQItems_ExtraWorkBoQId",
                table: "ExtraWorkBoQItems",
                column: "ExtraWorkBoQId");

            migrationBuilder.CreateIndex(
                name: "IX_ExtraWorkBoQItems_Install_UnitId",
                table: "ExtraWorkBoQItems",
                column: "Install_UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_ExtraWorkBoQItems_Rental_UnitId",
                table: "ExtraWorkBoQItems",
                column: "Rental_UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_ExtraWorkBoQItems_ReportTypeId",
                table: "ExtraWorkBoQItems",
                column: "ReportTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ExtraWorkBoQs_BoQHeadLineId",
                table: "ExtraWorkBoQs",
                column: "BoQHeadLineId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExtraWorkBoQDescriptions");

            migrationBuilder.DropTable(
                name: "ExtraWorkBoQItems");

            migrationBuilder.DropTable(
                name: "ExtraWorkBoQHeaders");

            migrationBuilder.DropTable(
                name: "ExtraWorkBoQs");
        }
    }
}
