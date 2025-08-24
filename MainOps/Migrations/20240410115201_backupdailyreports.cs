using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class backupdailyreports : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DailyReportBackups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DrId = table.Column<int>(nullable: false),
                    TitleId = table.Column<int>(nullable: false),
                    ProjectId = table.Column<int>(nullable: false),
                    short_Description = table.Column<string>(nullable: true),
                    tobepaid = table.Column<int>(nullable: true),
                    EnteredIntoDataBase = table.Column<DateTime>(nullable: true),
                    LastEditedInDataBase = table.Column<DateTime>(nullable: true),
                    HasPhotos = table.Column<bool>(nullable: false),
                    Report_Date = table.Column<DateTime>(nullable: false),
                    StartHour = table.Column<TimeSpan>(nullable: false),
                    EndHour = table.Column<TimeSpan>(nullable: false),
                    Work_Performed = table.Column<string>(maxLength: 1500, nullable: false),
                    Extra_Works = table.Column<string>(nullable: true),
                    DoneBy = table.Column<string>(nullable: true),
                    Signature = table.Column<string>(nullable: true),
                    Amount = table.Column<int>(nullable: false),
                    Machinery = table.Column<string>(nullable: true),
                    StandingTime = table.Column<TimeSpan>(nullable: true),
                    SafetyHours = table.Column<TimeSpan>(nullable: true),
                    Report_Checked = table.Column<bool>(nullable: false),
                    Checked_By = table.Column<string>(nullable: true),
                    SubProjectId = table.Column<int>(nullable: true),
                    OtherPeople = table.Column<string>(nullable: true),
                    OtherPeopleIDs = table.Column<string>(nullable: true),
                    VariationOrderId = table.Column<int>(nullable: true),
                    InvoiceDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyReportBackups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyReportBackups_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DailyReportBackups_SubProjects_SubProjectId",
                        column: x => x.SubProjectId,
                        principalTable: "SubProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DailyReportBackups_Titles_TitleId",
                        column: x => x.TitleId,
                        principalTable: "Titles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DailyReportBackups_BoQHeadLines_VariationOrderId",
                        column: x => x.VariationOrderId,
                        principalTable: "BoQHeadLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DailyReportBackups_ProjectId",
                table: "DailyReportBackups",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyReportBackups_SubProjectId",
                table: "DailyReportBackups",
                column: "SubProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyReportBackups_TitleId",
                table: "DailyReportBackups",
                column: "TitleId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyReportBackups_VariationOrderId",
                table: "DailyReportBackups",
                column: "VariationOrderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyReportBackups");
        }
    }
}
