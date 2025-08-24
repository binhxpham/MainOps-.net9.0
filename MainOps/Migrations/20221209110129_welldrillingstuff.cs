using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class welldrillingstuff : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Drill_Date",
                table: "Wells",
                newName: "Drill_Date_Start");

            migrationBuilder.AddColumn<DateTime>(
                name: "Drill_Date_End",
                table: "Wells",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "SoilSamples",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    sample_meter = table.Column<double>(nullable: true),
                    Odour = table.Column<string>(nullable: true),
                    IsWet = table.Column<bool>(nullable: false),
                    WellId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoilSamples", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SoilSamples_Wells_WellId",
                        column: x => x.WellId,
                        principalTable: "Wells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SoilSamples_WellId",
                table: "SoilSamples",
                column: "WellId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SoilSamples");

            migrationBuilder.DropColumn(
                name: "Drill_Date_End",
                table: "Wells");

            migrationBuilder.RenameColumn(
                name: "Drill_Date_Start",
                table: "Wells",
                newName: "Drill_Date");
        }
    }
}
