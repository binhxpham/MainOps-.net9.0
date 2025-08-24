using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class PostingAdd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Diets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FullName = table.Column<string>(nullable: true),
                    EmployeeNumber = table.Column<string>(nullable: true),
                    PayPeriod = table.Column<string>(nullable: true),
                    ProjectId = table.Column<int>(nullable: true),
                    Day1_start = table.Column<DateTime>(nullable: true),
                    Day2_start = table.Column<DateTime>(nullable: true),
                    Day3_start = table.Column<DateTime>(nullable: true),
                    Day4_start = table.Column<DateTime>(nullable: true),
                    Day5_start = table.Column<DateTime>(nullable: true),
                    Day6_start = table.Column<DateTime>(nullable: true),
                    Day1_end = table.Column<DateTime>(nullable: true),
                    Day2_end = table.Column<DateTime>(nullable: true),
                    Day3_end = table.Column<DateTime>(nullable: true),
                    Day4_end = table.Column<DateTime>(nullable: true),
                    Day5_end = table.Column<DateTime>(nullable: true),
                    Day6_end = table.Column<DateTime>(nullable: true),
                    WorkPlaceName1 = table.Column<string>(nullable: true),
                    WorkPlaceName2 = table.Column<string>(nullable: true),
                    WorkPlaceName3 = table.Column<string>(nullable: true),
                    WorkPlaceName4 = table.Column<string>(nullable: true),
                    WorkPlaceName5 = table.Column<string>(nullable: true),
                    WorkPlaceName6 = table.Column<string>(nullable: true),
                    SelfContainedExpenses = table.Column<decimal>(nullable: true),
                    LivingInCamperWagon = table.Column<decimal>(nullable: true),
                    CalculationOfDietsAndSmallNecessities = table.Column<decimal>(nullable: true),
                    HourAddon = table.Column<decimal>(nullable: true),
                    DeductionBreakFast = table.Column<decimal>(nullable: true),
                    DeductionLunch = table.Column<decimal>(nullable: true),
                    DeductionDinner = table.Column<decimal>(nullable: true),
                    SignatureEmployee = table.Column<string>(nullable: true),
                    SignatureSupervisor = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Diets_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Diets_ProjectId",
                table: "Diets",
                column: "ProjectId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Diets");
        }
    }
}
