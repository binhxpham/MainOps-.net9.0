using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class acidtreatments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AcidTreatments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ProjectId = table.Column<int>(nullable: false),
                    SubProjectId = table.Column<int>(nullable: true),
                    Wellname = table.Column<string>(nullable: true),
                    MeasPointId = table.Column<int>(nullable: true),
                    Report_Date = table.Column<DateTime>(nullable: false),
                    Water_Meter_Before = table.Column<double>(nullable: true),
                    starttime = table.Column<TimeSpan>(nullable: false),
                    endtime = table.Column<TimeSpan>(nullable: false),
                    Water_Meter_After = table.Column<double>(nullable: true),
                    Bottom_well = table.Column<double>(nullable: true),
                    Water_level = table.Column<double>(nullable: true),
                    Coments = table.Column<string>(nullable: true),
                    DoneBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcidTreatments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AcidTreatments_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AcidTreatments_SubProjects_SubProjectId",
                        column: x => x.SubProjectId,
                        principalTable: "SubProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AcidDatas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TimeStamp = table.Column<TimeSpan>(nullable: false),
                    Pressure = table.Column<double>(nullable: true),
                    Min_Counter_Water = table.Column<double>(nullable: true),
                    Min_Counter_HoseCounter = table.Column<double>(nullable: true),
                    Hour_Counter_Water = table.Column<double>(nullable: true),
                    Hour_Counter_HoseCounter = table.Column<double>(nullable: true),
                    Acid_m3_total = table.Column<double>(nullable: true),
                    m3_total = table.Column<double>(nullable: true),
                    Set_Dosing_Time = table.Column<double>(nullable: true),
                    Set_Dosing_Percent = table.Column<double>(nullable: true),
                    Set_Dosing_m3 = table.Column<double>(nullable: true),
                    Level = table.Column<double>(nullable: true),
                    Flow = table.Column<double>(nullable: true),
                    AcidTreatmentId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcidDatas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AcidDatas_AcidTreatments_AcidTreatmentId",
                        column: x => x.AcidTreatmentId,
                        principalTable: "AcidTreatments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AcidDatas_AcidTreatmentId",
                table: "AcidDatas",
                column: "AcidTreatmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AcidTreatments_ProjectId",
                table: "AcidTreatments",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_AcidTreatments_SubProjectId",
                table: "AcidTreatments",
                column: "SubProjectId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AcidDatas");

            migrationBuilder.DropTable(
                name: "AcidTreatments");
        }
    }
}
