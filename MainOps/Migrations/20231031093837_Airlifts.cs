using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class Airlifts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Airlifts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ProjectId = table.Column<int>(nullable: false),
                    SubProjectId = table.Column<int>(nullable: true),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    BottomWellBefore = table.Column<double>(nullable: true),
                    WaterLevelBefore = table.Column<double>(nullable: true),
                    BottomWellAfter = table.Column<double>(nullable: true),
                    WaterLevelAfter = table.Column<double>(nullable: true),
                    MeasPointId = table.Column<int>(nullable: true),
                    WellName = table.Column<string>(nullable: true),
                    Comments = table.Column<string>(nullable: true),
                    DoneBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Airlifts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Airlifts_MeasPoints_MeasPointId",
                        column: x => x.MeasPointId,
                        principalTable: "MeasPoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AirliftPhotos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AirliftId = table.Column<int>(nullable: false),
                    path = table.Column<string>(nullable: true),
                    TimeStamp = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AirliftPhotos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AirliftPhotos_Airlifts_AirliftId",
                        column: x => x.AirliftId,
                        principalTable: "Airlifts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AirliftPhotos_AirliftId",
                table: "AirliftPhotos",
                column: "AirliftId");

            migrationBuilder.CreateIndex(
                name: "IX_Airlifts_MeasPointId",
                table: "Airlifts",
                column: "MeasPointId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AirliftPhotos");

            migrationBuilder.DropTable(
                name: "Airlifts");
        }
    }
}
