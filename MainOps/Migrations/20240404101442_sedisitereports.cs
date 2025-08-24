using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class sedisitereports : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SedimentationSiteReports",
                columns: table => new
                {
                    ProjectId = table.Column<int>(nullable: false),
                    SubProjectId = table.Column<int>(nullable: true),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    Comments = table.Column<string>(nullable: true),
                    DoneBy = table.Column<string>(nullable: true),
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SedimenationCleanedAndEmptied = table.Column<bool>(nullable: false),
                    SedimentationShouldBeEmptiedAndCleaned = table.Column<bool>(nullable: false),
                    GeoTubeExchanged = table.Column<bool>(nullable: false),
                    GeoTubeShouldBeExchanged = table.Column<bool>(nullable: false),
                    SiteCheckPerformed = table.Column<bool>(nullable: false),
                    Leakages = table.Column<bool>(nullable: false),
                    AccessWays = table.Column<bool>(nullable: false),
                    Safety = table.Column<bool>(nullable: false),
                    AcidExchanged = table.Column<bool>(nullable: false),
                    AcidShouldBeExchanged = table.Column<bool>(nullable: false),
                    AlarmFunction = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SedimentationSiteReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SedimentationSiteReports_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SedimentationSiteReports_SubProjects_SubProjectId",
                        column: x => x.SubProjectId,
                        principalTable: "SubProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SedimentationSiteReportPhotos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SedimentationSiteReportId = table.Column<int>(nullable: false),
                    Path = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SedimentationSiteReportPhotos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SedimentationSiteReportPhotos_SedimentationSiteReports_Sedim~",
                        column: x => x.SedimentationSiteReportId,
                        principalTable: "SedimentationSiteReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SedimentationSiteReportPhotos_SedimentationSiteReportId",
                table: "SedimentationSiteReportPhotos",
                column: "SedimentationSiteReportId");

            migrationBuilder.CreateIndex(
                name: "IX_SedimentationSiteReports_ProjectId",
                table: "SedimentationSiteReports",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_SedimentationSiteReports_SubProjectId",
                table: "SedimentationSiteReports",
                column: "SubProjectId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SedimentationSiteReportPhotos");

            migrationBuilder.DropTable(
                name: "SedimentationSiteReports");
        }
    }
}
