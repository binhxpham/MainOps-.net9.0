using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class ongoinghourregistrations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HourRegistrations_Ongoing",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Week1 = table.Column<int>(nullable: false),
                    Week2 = table.Column<int>(nullable: false),
                    FullName = table.Column<string>(nullable: true),
                    LicensePlate = table.Column<string>(nullable: true),
                    PaymentNr = table.Column<string>(nullable: true),
                    Signature_Worker = table.Column<string>(nullable: true),
                    Signature_Supervisor = table.Column<string>(nullable: true),
                    Supervisor_Name = table.Column<string>(nullable: true),
                    weektype = table.Column<string>(nullable: true),
                    totalregularhours = table.Column<double>(nullable: false),
                    totaloverhours50 = table.Column<double>(nullable: true),
                    totaloverhours100 = table.Column<double>(nullable: true),
                    totaladdons = table.Column<double>(nullable: true),
                    Edited = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HourRegistrations_Ongoing", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RowHours_Ongoing",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Edited = table.Column<bool>(nullable: false),
                    HourRegistration_OngoingId = table.Column<int>(nullable: true),
                    day1 = table.Column<double>(nullable: true),
                    day2 = table.Column<double>(nullable: true),
                    day3 = table.Column<double>(nullable: true),
                    day4 = table.Column<double>(nullable: true),
                    day5 = table.Column<double>(nullable: true),
                    day6 = table.Column<double>(nullable: true),
                    day7 = table.Column<double>(nullable: true),
                    day8 = table.Column<double>(nullable: true),
                    day9 = table.Column<double>(nullable: true),
                    day10 = table.Column<double>(nullable: true),
                    day11 = table.Column<double>(nullable: true),
                    day12 = table.Column<double>(nullable: true),
                    day13 = table.Column<double>(nullable: true),
                    day14 = table.Column<double>(nullable: true),
                    day1_Alarm = table.Column<double>(nullable: true),
                    day2_Alarm = table.Column<double>(nullable: true),
                    day3_Alarm = table.Column<double>(nullable: true),
                    day4_Alarm = table.Column<double>(nullable: true),
                    day5_Alarm = table.Column<double>(nullable: true),
                    day6_Alarm = table.Column<double>(nullable: true),
                    day7_Alarm = table.Column<double>(nullable: true),
                    day8_Alarm = table.Column<double>(nullable: true),
                    day9_Alarm = table.Column<double>(nullable: true),
                    day10_Alarm = table.Column<double>(nullable: true),
                    day11_Alarm = table.Column<double>(nullable: true),
                    day12_Alarm = table.Column<double>(nullable: true),
                    day13_Alarm = table.Column<double>(nullable: true),
                    day14_Alarm = table.Column<double>(nullable: true),
                    day1_Type = table.Column<string>(nullable: true),
                    day2_Type = table.Column<string>(nullable: true),
                    day3_Type = table.Column<string>(nullable: true),
                    day4_Type = table.Column<string>(nullable: true),
                    day5_Type = table.Column<string>(nullable: true),
                    day6_Type = table.Column<string>(nullable: true),
                    day7_Type = table.Column<string>(nullable: true),
                    day8_Type = table.Column<string>(nullable: true),
                    day9_Type = table.Column<string>(nullable: true),
                    day10_Type = table.Column<string>(nullable: true),
                    day11_Type = table.Column<string>(nullable: true),
                    day12_Type = table.Column<string>(nullable: true),
                    day13_Type = table.Column<string>(nullable: true),
                    day14_Type = table.Column<string>(nullable: true),
                    day1314_Alarm = table.Column<double>(nullable: true),
                    ProjectId = table.Column<int>(nullable: true),
                    Others = table.Column<string>(nullable: true),
                    OverHours_50 = table.Column<double>(nullable: true),
                    OverHours_100 = table.Column<double>(nullable: true),
                    AddOns = table.Column<int>(nullable: true),
                    AddOns_Amount = table.Column<double>(nullable: true),
                    Skur_penge = table.Column<double>(nullable: true),
                    Driving_Money_Days = table.Column<int>(nullable: true),
                    Driving_Money_Amount = table.Column<double>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RowHours_Ongoing", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RowHours_Ongoing_HourRegistrations_Ongoing_HourRegistration_~",
                        column: x => x.HourRegistration_OngoingId,
                        principalTable: "HourRegistrations_Ongoing",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RowHours_Ongoing_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RowHours_Ongoing_HourRegistration_OngoingId",
                table: "RowHours_Ongoing",
                column: "HourRegistration_OngoingId");

            migrationBuilder.CreateIndex(
                name: "IX_RowHours_Ongoing_ProjectId",
                table: "RowHours_Ongoing",
                column: "ProjectId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RowHours_Ongoing");

            migrationBuilder.DropTable(
                name: "HourRegistrations_Ongoing");
        }
    }
}
