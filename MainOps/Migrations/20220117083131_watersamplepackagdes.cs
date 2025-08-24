using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class watersamplepackagdes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WaterSamplePackages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ProjectId = table.Column<int>(nullable: true),
                    Annotation = table.Column<string>(nullable: true),
                    ListOfComponents = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaterSamplePackages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WaterSamplePackages_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WaterSampleTypeWaterSamplePackages",
                columns: table => new
                {
                    WaterSamplePackageId = table.Column<int>(nullable: false),
                    WaterSampleTypeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaterSampleTypeWaterSamplePackages", x => new { x.WaterSamplePackageId, x.WaterSampleTypeId });
                    table.ForeignKey(
                        name: "FK_WaterSampleTypeWaterSamplePackages_WaterSamplePackages_Water~",
                        column: x => x.WaterSamplePackageId,
                        principalTable: "WaterSamplePackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WaterSampleTypeWaterSamplePackages_WaterSampleTypes_WaterSam~",
                        column: x => x.WaterSampleTypeId,
                        principalTable: "WaterSampleTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WaterSamplePackages_ProjectId",
                table: "WaterSamplePackages",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_WaterSampleTypeWaterSamplePackages_WaterSampleTypeId",
                table: "WaterSampleTypeWaterSamplePackages",
                column: "WaterSampleTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WaterSampleTypeWaterSamplePackages");

            migrationBuilder.DropTable(
                name: "WaterSamplePackages");
        }
    }
}
