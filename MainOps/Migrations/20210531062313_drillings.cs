using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class drillings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WTP_blocks_Item_Category_item_categoryid",
                table: "WTP_blocks");

            migrationBuilder.DropTable(
                name: "Item_Category");

            migrationBuilder.DropIndex(
                name: "IX_WTP_blocks_item_categoryid",
                table: "WTP_blocks");

            migrationBuilder.DropColumn(
                name: "item_categoryid",
                table: "WTP_blocks");

            migrationBuilder.DropColumn(
                name: "ger_name",
                table: "Prices");

            migrationBuilder.CreateTable(
                name: "Drillings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    WellID = table.Column<string>(nullable: true),
                    DrillDepth = table.Column<double>(nullable: false),
                    PipeDiameter = table.Column<string>(nullable: true),
                    FilterLength = table.Column<double>(nullable: false),
                    BlindPipeDepth = table.Column<double>(nullable: false),
                    SandBagsUsed = table.Column<double>(nullable: false),
                    MikrolitBagsUsed = table.Column<double>(nullable: false),
                    Latitude = table.Column<double>(nullable: false),
                    Longitude = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drillings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DrillPhotos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Path = table.Column<string>(nullable: true),
                    DrillId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DrillPhotos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DrillPhotos_Drillings_DrillId",
                        column: x => x.DrillId,
                        principalTable: "Drillings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DrillPhotos_DrillId",
                table: "DrillPhotos",
                column: "DrillId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DrillPhotos");

            migrationBuilder.DropTable(
                name: "Drillings");

            migrationBuilder.AddColumn<int>(
                name: "item_categoryid",
                table: "WTP_blocks",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ger_name",
                table: "Prices",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Item_Category",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Item_category = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Item_Category", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WTP_blocks_item_categoryid",
                table: "WTP_blocks",
                column: "item_categoryid");

            migrationBuilder.AddForeignKey(
                name: "FK_WTP_blocks_Item_Category_item_categoryid",
                table: "WTP_blocks",
                column: "item_categoryid",
                principalTable: "Item_Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
