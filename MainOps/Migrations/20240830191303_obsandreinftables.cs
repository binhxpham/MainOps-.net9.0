using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class obsandreinftables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ObservationInstallation",
                columns: table => new
                {
                    ProjectId = table.Column<int>(nullable: false),
                    SubProjectId = table.Column<int>(nullable: true),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    Comments = table.Column<string>(nullable: true),
                    DoneBy = table.Column<string>(nullable: true),
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MeasPointId = table.Column<int>(nullable: true),
                    WellName = table.Column<string>(nullable: true),
                    ObservationTypeWritten = table.Column<string>(nullable: true),
                    SensorRange = table.Column<string>(nullable: true),
                    WellDepth = table.Column<double>(nullable: true),
                    SensorDepth = table.Column<double>(nullable: true),
                    WaterLevel = table.Column<double>(nullable: true),
                    VariationOrderId = table.Column<int>(nullable: true),
                    Latitude = table.Column<double>(nullable: true),
                    Longitude = table.Column<double>(nullable: true),
                    Accuracy = table.Column<double>(nullable: true),
                    PipeCut = table.Column<double>(nullable: true),
                    ObservationTypeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObservationInstallation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObservationInstallation_MeasPoints_MeasPointId",
                        column: x => x.MeasPointId,
                        principalTable: "MeasPoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ObservationInstallation_ItemTypes_ObservationTypeId",
                        column: x => x.ObservationTypeId,
                        principalTable: "ItemTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ObservationInstallation_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ObservationInstallation_SubProjects_SubProjectId",
                        column: x => x.SubProjectId,
                        principalTable: "SubProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ObservationInstallation_BoQHeadLines_VariationOrderId",
                        column: x => x.VariationOrderId,
                        principalTable: "BoQHeadLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReinfiltrationInstallation",
                columns: table => new
                {
                    ProjectId = table.Column<int>(nullable: false),
                    SubProjectId = table.Column<int>(nullable: true),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    Comments = table.Column<string>(nullable: true),
                    DoneBy = table.Column<string>(nullable: true),
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MeasPointId = table.Column<int>(nullable: true),
                    WellName = table.Column<string>(nullable: true),
                    ReinfiltrationTypeWritten = table.Column<string>(nullable: true),
                    SensorRange = table.Column<string>(nullable: true),
                    WellDepth = table.Column<double>(nullable: true),
                    SensorDepth = table.Column<double>(nullable: true),
                    DiameterHose = table.Column<string>(nullable: true),
                    WaterLevel = table.Column<double>(nullable: true),
                    VariationOrderId = table.Column<int>(nullable: true),
                    Latitude = table.Column<double>(nullable: true),
                    Longitude = table.Column<double>(nullable: true),
                    Accuracy = table.Column<double>(nullable: true),
                    ReinfiltrationTypeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReinfiltrationInstallation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReinfiltrationInstallation_MeasPoints_MeasPointId",
                        column: x => x.MeasPointId,
                        principalTable: "MeasPoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReinfiltrationInstallation_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReinfiltrationInstallation_ItemTypes_ReinfiltrationTypeId",
                        column: x => x.ReinfiltrationTypeId,
                        principalTable: "ItemTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReinfiltrationInstallation_SubProjects_SubProjectId",
                        column: x => x.SubProjectId,
                        principalTable: "SubProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReinfiltrationInstallation_BoQHeadLines_VariationOrderId",
                        column: x => x.VariationOrderId,
                        principalTable: "BoQHeadLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ObservationInstallation_MeasPointId",
                table: "ObservationInstallation",
                column: "MeasPointId");

            migrationBuilder.CreateIndex(
                name: "IX_ObservationInstallation_ObservationTypeId",
                table: "ObservationInstallation",
                column: "ObservationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ObservationInstallation_ProjectId",
                table: "ObservationInstallation",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ObservationInstallation_SubProjectId",
                table: "ObservationInstallation",
                column: "SubProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ObservationInstallation_VariationOrderId",
                table: "ObservationInstallation",
                column: "VariationOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ReinfiltrationInstallation_MeasPointId",
                table: "ReinfiltrationInstallation",
                column: "MeasPointId");

            migrationBuilder.CreateIndex(
                name: "IX_ReinfiltrationInstallation_ProjectId",
                table: "ReinfiltrationInstallation",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ReinfiltrationInstallation_ReinfiltrationTypeId",
                table: "ReinfiltrationInstallation",
                column: "ReinfiltrationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ReinfiltrationInstallation_SubProjectId",
                table: "ReinfiltrationInstallation",
                column: "SubProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ReinfiltrationInstallation_VariationOrderId",
                table: "ReinfiltrationInstallation",
                column: "VariationOrderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ObservationInstallation");

            migrationBuilder.DropTable(
                name: "ReinfiltrationInstallation");
        }
    }
}
