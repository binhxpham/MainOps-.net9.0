using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class truckerdaily : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TruckDailyReports",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Dato = table.Column<DateTime>(nullable: false),
                    DoneBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TruckDailyReports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TruckSites",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ProjectId = table.Column<int>(nullable: true),
                    SubProjectId = table.Column<int>(nullable: true),
                    Hours = table.Column<TimeSpan>(nullable: false),
                    TruckDailyReportId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TruckSites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TruckSites_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TruckSites_SubProjects_SubProjectId",
                        column: x => x.SubProjectId,
                        principalTable: "SubProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TruckSites_TruckDailyReports_TruckDailyReportId",
                        column: x => x.TruckDailyReportId,
                        principalTable: "TruckDailyReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TruckSites_ProjectId",
                table: "TruckSites",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TruckSites_SubProjectId",
                table: "TruckSites",
                column: "SubProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TruckSites_TruckDailyReportId",
                table: "TruckSites",
                column: "TruckDailyReportId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TruckSites");

            migrationBuilder.DropTable(
                name: "TruckDailyReports");
        }
    }
}
