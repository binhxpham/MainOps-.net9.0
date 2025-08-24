using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class pumpinstallations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PumpInstallation",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ProjectId = table.Column<int>(nullable: false),
                    SubProjectId = table.Column<int>(nullable: true),
                    MeasPointId = table.Column<int>(nullable: true),
                    WellName = table.Column<string>(nullable: true),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    PumpTypeWritten = table.Column<string>(nullable: true),
                    SensorRange = table.Column<string>(nullable: true),
                    WellDepth = table.Column<double>(nullable: true),
                    PumpDepth = table.Column<double>(nullable: true),
                    SensorDepth = table.Column<double>(nullable: true),
                    DiameterHose = table.Column<string>(nullable: true),
                    WaterLevel = table.Column<double>(nullable: true),
                    PipeCut = table.Column<double>(nullable: true),
                    Comments = table.Column<string>(nullable: true),
                    VariationOrderId = table.Column<int>(nullable: true),
                    Latitude = table.Column<double>(nullable: true),
                    Longitude = table.Column<double>(nullable: true),
                    Accuracy = table.Column<double>(nullable: true),
                    PumpTypeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PumpInstallation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PumpInstallation_MeasPoints_MeasPointId",
                        column: x => x.MeasPointId,
                        principalTable: "MeasPoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PumpInstallation_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PumpInstallation_ItemTypes_PumpTypeId",
                        column: x => x.PumpTypeId,
                        principalTable: "ItemTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PumpInstallation_SubProjects_SubProjectId",
                        column: x => x.SubProjectId,
                        principalTable: "SubProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PumpInstallation_BoQHeadLines_VariationOrderId",
                        column: x => x.VariationOrderId,
                        principalTable: "BoQHeadLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PumpInstallation_MeasPointId",
                table: "PumpInstallation",
                column: "MeasPointId");

            migrationBuilder.CreateIndex(
                name: "IX_PumpInstallation_ProjectId",
                table: "PumpInstallation",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_PumpInstallation_PumpTypeId",
                table: "PumpInstallation",
                column: "PumpTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PumpInstallation_SubProjectId",
                table: "PumpInstallation",
                column: "SubProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_PumpInstallation_VariationOrderId",
                table: "PumpInstallation",
                column: "VariationOrderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PumpInstallation");
        }
    }
}
