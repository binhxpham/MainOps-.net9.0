using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class pumptesting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PumpTestings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DoneBy = table.Column<string>(nullable: true),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    PumpType = table.Column<string>(nullable: true),
                    PumpID = table.Column<string>(nullable: true),
                    CableLength = table.Column<double>(nullable: false),
                    CableDamaged = table.Column<bool>(nullable: false),
                    WhichCableDamaged = table.Column<string>(nullable: true),
                    CanBeFixed = table.Column<bool>(nullable: false),
                    CompleteExchange = table.Column<bool>(nullable: false),
                    CableExtended = table.Column<bool>(nullable: false),
                    CableExtendedm = table.Column<double>(nullable: false),
                    InsulationTested = table.Column<bool>(nullable: false),
                    InsulationOK = table.Column<bool>(nullable: false),
                    AllCableSubmerged = table.Column<bool>(nullable: false),
                    Signature = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PumpTestings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PumptestingDatas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PumpTestingId = table.Column<int>(nullable: true),
                    Pressure = table.Column<double>(nullable: false),
                    Flow = table.Column<double>(nullable: false),
                    Duration = table.Column<TimeSpan>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PumptestingDatas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PumptestingDatas_PumpTestings_PumpTestingId",
                        column: x => x.PumpTestingId,
                        principalTable: "PumpTestings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PumptestingDatas_PumpTestingId",
                table: "PumptestingDatas",
                column: "PumpTestingId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PumptestingDatas");

            migrationBuilder.DropTable(
                name: "PumpTestings");
        }
    }
}
