using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class handlethatwater : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DrillWaters",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ProjectId = table.Column<int>(nullable: false),
                    SubProjectId = table.Column<int>(nullable: true),
                    MeasPointId = table.Column<int>(nullable: true),
                    DrillWaterStart = table.Column<double>(nullable: true),
                    DrillWaterEnd = table.Column<double>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DrillWaters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DrillWaters_MeasPoints_MeasPointId",
                        column: x => x.MeasPointId,
                        principalTable: "MeasPoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DrillWaters_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DrillWaters_SubProjects_SubProjectId",
                        column: x => x.SubProjectId,
                        principalTable: "SubProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DrillWaterPhotos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    path = table.Column<string>(nullable: true),
                    DrillWaterId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DrillWaterPhotos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DrillWaterPhotos_DrillWaters_DrillWaterId",
                        column: x => x.DrillWaterId,
                        principalTable: "DrillWaters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WaterHandlings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MeasPointId = table.Column<int>(nullable: true),
                    ClearPumpTestId = table.Column<int>(nullable: true),
                    DrillWaterId = table.Column<int>(nullable: true),
                    WellName = table.Column<string>(nullable: true),
                    DrillId = table.Column<int>(nullable: true),
                    PumpWaterStart = table.Column<double>(nullable: true),
                    PumpWaterEnd = table.Column<double>(nullable: true),
                    DrillWaterStart = table.Column<double>(nullable: true),
                    DrillWaterEnd = table.Column<double>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaterHandlings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WaterHandlings_ClearPumpTests_ClearPumpTestId",
                        column: x => x.ClearPumpTestId,
                        principalTable: "ClearPumpTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WaterHandlings_Drillings_DrillId",
                        column: x => x.DrillId,
                        principalTable: "Drillings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WaterHandlings_DrillWaters_DrillWaterId",
                        column: x => x.DrillWaterId,
                        principalTable: "DrillWaters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WaterHandlings_MeasPoints_MeasPointId",
                        column: x => x.MeasPointId,
                        principalTable: "MeasPoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DrillWaterPhotos_DrillWaterId",
                table: "DrillWaterPhotos",
                column: "DrillWaterId");

            migrationBuilder.CreateIndex(
                name: "IX_DrillWaters_MeasPointId",
                table: "DrillWaters",
                column: "MeasPointId");

            migrationBuilder.CreateIndex(
                name: "IX_DrillWaters_ProjectId",
                table: "DrillWaters",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_DrillWaters_SubProjectId",
                table: "DrillWaters",
                column: "SubProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_WaterHandlings_ClearPumpTestId",
                table: "WaterHandlings",
                column: "ClearPumpTestId");

            migrationBuilder.CreateIndex(
                name: "IX_WaterHandlings_DrillId",
                table: "WaterHandlings",
                column: "DrillId");

            migrationBuilder.CreateIndex(
                name: "IX_WaterHandlings_DrillWaterId",
                table: "WaterHandlings",
                column: "DrillWaterId");

            migrationBuilder.CreateIndex(
                name: "IX_WaterHandlings_MeasPointId",
                table: "WaterHandlings",
                column: "MeasPointId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DrillWaterPhotos");

            migrationBuilder.DropTable(
                name: "WaterHandlings");

            migrationBuilder.DropTable(
                name: "DrillWaters");
        }
    }
}
