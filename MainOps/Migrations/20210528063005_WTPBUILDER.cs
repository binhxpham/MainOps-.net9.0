using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class WTPBUILDER : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WTP_blockid",
                table: "Documents",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Atoms",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 50, nullable: false),
                    mass = table.Column<double>(nullable: false),
                    symbol = table.Column<string>(maxLength: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Atoms", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    category = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Dosings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    dosing = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dosings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Effect_types",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    description = table.Column<string>(nullable: false),
                    path_to_graph = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Effect_types", x => x.id);
                });

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

            migrationBuilder.CreateTable(
                name: "Luxurities",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    wtp_luxurity = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Luxurities", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Special_Cases_Air_Strippers",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    cont_name = table.Column<string>(nullable: true),
                    new_filter = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Special_Cases_Air_Strippers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Temporal_sections",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    section = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Temporal_sections", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Water_types",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    water_type = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Water_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "WTPUnits",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    the_unit = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WTPUnits", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "FilterMaterials",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 120, nullable: false),
                    contaminations = table.Column<string>(nullable: false),
                    device = table.Column<string>(nullable: false),
                    water_typeid = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilterMaterials", x => x.id);
                    table.ForeignKey(
                        name: "FK_FilterMaterials_Water_types_water_typeid",
                        column: x => x.water_typeid,
                        principalTable: "Water_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Contaminations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    contam_group = table.Column<string>(maxLength: 25, nullable: false),
                    default_limit = table.Column<double>(nullable: true),
                    Unit_limitid = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contaminations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contaminations_WTPUnits_Unit_limitid",
                        column: x => x.Unit_limitid,
                        principalTable: "WTPUnits",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Efforts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    WTP_block_name = table.Column<string>(maxLength: 80, nullable: false),
                    CategoryId = table.Column<int>(nullable: true),
                    effort = table.Column<double>(nullable: false),
                    WTPUnitId = table.Column<int>(nullable: true),
                    Temp_sectionid = table.Column<int>(nullable: true),
                    DivisionId = table.Column<int>(nullable: true),
                    Wtp_luxurityId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Efforts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Efforts_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Efforts_Divisions_DivisionId",
                        column: x => x.DivisionId,
                        principalTable: "Divisions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Efforts_Temporal_sections_Temp_sectionid",
                        column: x => x.Temp_sectionid,
                        principalTable: "Temporal_sections",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Efforts_WTPUnits_WTPUnitId",
                        column: x => x.WTPUnitId,
                        principalTable: "WTPUnits",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Efforts_Luxurities_Wtp_luxurityId",
                        column: x => x.Wtp_luxurityId,
                        principalTable: "Luxurities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Prices",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 100, nullable: true),
                    ger_name = table.Column<string>(maxLength: 100, nullable: true),
                    EkdTid = table.Column<int>(nullable: false),
                    size = table.Column<double>(nullable: true),
                    unitid = table.Column<int>(nullable: false),
                    price = table.Column<double>(nullable: true),
                    unit_pid = table.Column<int>(nullable: false),
                    unit_rid = table.Column<int>(nullable: false),
                    rent = table.Column<double>(nullable: true),
                    divisionid = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prices", x => x.id);
                    table.ForeignKey(
                        name: "FK_Prices_Categories_EkdTid",
                        column: x => x.EkdTid,
                        principalTable: "Categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Prices_Divisions_divisionid",
                        column: x => x.divisionid,
                        principalTable: "Divisions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Prices_WTPUnits_unit_pid",
                        column: x => x.unit_pid,
                        principalTable: "WTPUnits",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Prices_WTPUnits_unit_rid",
                        column: x => x.unit_rid,
                        principalTable: "WTPUnits",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Prices_WTPUnits_unitid",
                        column: x => x.unitid,
                        principalTable: "WTPUnits",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WTP_blocks",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 100, nullable: true),
                    size = table.Column<double>(nullable: false),
                    unit_sizeid = table.Column<int>(nullable: true),
                    length = table.Column<double>(nullable: true),
                    width = table.Column<double>(nullable: true),
                    height = table.Column<double>(nullable: true),
                    weight = table.Column<double>(nullable: true),
                    Pow_Con = table.Column<double>(nullable: true),
                    WTPUnitId = table.Column<int>(nullable: true),
                    necessity = table.Column<bool>(nullable: false),
                    item_categoryid = table.Column<int>(nullable: false),
                    DivisionId = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WTP_blocks", x => x.id);
                    table.ForeignKey(
                        name: "FK_WTP_blocks_Divisions_DivisionId",
                        column: x => x.DivisionId,
                        principalTable: "Divisions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WTP_blocks_WTPUnits_WTPUnitId",
                        column: x => x.WTPUnitId,
                        principalTable: "WTPUnits",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WTP_blocks_Item_Category_item_categoryid",
                        column: x => x.item_categoryid,
                        principalTable: "Item_Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WTP_blocks_WTPUnits_unit_sizeid",
                        column: x => x.unit_sizeid,
                        principalTable: "WTPUnits",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MediaEfficiencies",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    filtermaterialid = table.Column<int>(nullable: false),
                    contaminationId = table.Column<int>(nullable: true),
                    efficiency = table.Column<sbyte>(nullable: false),
                    dosing_ofid = table.Column<int>(nullable: true),
                    need_dosing = table.Column<bool>(nullable: false),
                    dosing_relation = table.Column<double>(nullable: false),
                    need_Aeration = table.Column<bool>(nullable: false),
                    lower_limit_aeration = table.Column<double>(nullable: false),
                    upper_limit_aeration = table.Column<double>(nullable: false),
                    need_pH_control = table.Column<bool>(nullable: false),
                    lower_limit_pH = table.Column<double>(nullable: false),
                    upper_limit_pH = table.Column<double>(nullable: false),
                    has_concentration_effect = table.Column<bool>(nullable: false),
                    effect_typeid = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaEfficiencies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MediaEfficiencies_Contaminations_contaminationId",
                        column: x => x.contaminationId,
                        principalTable: "Contaminations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MediaEfficiencies_Dosings_dosing_ofid",
                        column: x => x.dosing_ofid,
                        principalTable: "Dosings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MediaEfficiencies_Effect_types_effect_typeid",
                        column: x => x.effect_typeid,
                        principalTable: "Effect_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MediaEfficiencies_FilterMaterials_filtermaterialid",
                        column: x => x.filtermaterialid,
                        principalTable: "FilterMaterials",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Documents_WTP_blockid",
                table: "Documents",
                column: "WTP_blockid");

            migrationBuilder.CreateIndex(
                name: "IX_Contaminations_Unit_limitid",
                table: "Contaminations",
                column: "Unit_limitid");

            migrationBuilder.CreateIndex(
                name: "IX_Efforts_CategoryId",
                table: "Efforts",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Efforts_DivisionId",
                table: "Efforts",
                column: "DivisionId");

            migrationBuilder.CreateIndex(
                name: "IX_Efforts_Temp_sectionid",
                table: "Efforts",
                column: "Temp_sectionid");

            migrationBuilder.CreateIndex(
                name: "IX_Efforts_WTPUnitId",
                table: "Efforts",
                column: "WTPUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Efforts_Wtp_luxurityId",
                table: "Efforts",
                column: "Wtp_luxurityId");

            migrationBuilder.CreateIndex(
                name: "IX_FilterMaterials_water_typeid",
                table: "FilterMaterials",
                column: "water_typeid");

            migrationBuilder.CreateIndex(
                name: "IX_MediaEfficiencies_contaminationId",
                table: "MediaEfficiencies",
                column: "contaminationId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaEfficiencies_dosing_ofid",
                table: "MediaEfficiencies",
                column: "dosing_ofid");

            migrationBuilder.CreateIndex(
                name: "IX_MediaEfficiencies_effect_typeid",
                table: "MediaEfficiencies",
                column: "effect_typeid");

            migrationBuilder.CreateIndex(
                name: "IX_MediaEfficiencies_filtermaterialid",
                table: "MediaEfficiencies",
                column: "filtermaterialid");

            migrationBuilder.CreateIndex(
                name: "IX_Prices_EkdTid",
                table: "Prices",
                column: "EkdTid");

            migrationBuilder.CreateIndex(
                name: "IX_Prices_divisionid",
                table: "Prices",
                column: "divisionid");

            migrationBuilder.CreateIndex(
                name: "IX_Prices_unit_pid",
                table: "Prices",
                column: "unit_pid");

            migrationBuilder.CreateIndex(
                name: "IX_Prices_unit_rid",
                table: "Prices",
                column: "unit_rid");

            migrationBuilder.CreateIndex(
                name: "IX_Prices_unitid",
                table: "Prices",
                column: "unitid");

            migrationBuilder.CreateIndex(
                name: "IX_WTP_blocks_DivisionId",
                table: "WTP_blocks",
                column: "DivisionId");

            migrationBuilder.CreateIndex(
                name: "IX_WTP_blocks_WTPUnitId",
                table: "WTP_blocks",
                column: "WTPUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_WTP_blocks_item_categoryid",
                table: "WTP_blocks",
                column: "item_categoryid");

            migrationBuilder.CreateIndex(
                name: "IX_WTP_blocks_unit_sizeid",
                table: "WTP_blocks",
                column: "unit_sizeid");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_WTP_blocks_WTP_blockid",
                table: "Documents",
                column: "WTP_blockid",
                principalTable: "WTP_blocks",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_WTP_blocks_WTP_blockid",
                table: "Documents");

            migrationBuilder.DropTable(
                name: "Atoms");

            migrationBuilder.DropTable(
                name: "Efforts");

            migrationBuilder.DropTable(
                name: "MediaEfficiencies");

            migrationBuilder.DropTable(
                name: "Prices");

            migrationBuilder.DropTable(
                name: "Special_Cases_Air_Strippers");

            migrationBuilder.DropTable(
                name: "WTP_blocks");

            migrationBuilder.DropTable(
                name: "Temporal_sections");

            migrationBuilder.DropTable(
                name: "Luxurities");

            migrationBuilder.DropTable(
                name: "Contaminations");

            migrationBuilder.DropTable(
                name: "Dosings");

            migrationBuilder.DropTable(
                name: "Effect_types");

            migrationBuilder.DropTable(
                name: "FilterMaterials");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Item_Category");

            migrationBuilder.DropTable(
                name: "WTPUnits");

            migrationBuilder.DropTable(
                name: "Water_types");

            migrationBuilder.DropIndex(
                name: "IX_Documents_WTP_blockid",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "WTP_blockid",
                table: "Documents");
        }
    }
}
