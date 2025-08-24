using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class wellchecks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WellChecks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    WellName = table.Column<string>(nullable: true),
                    CanBeFound = table.Column<bool>(nullable: false),
                    Comments = table.Column<string>(nullable: true),
                    Dip = table.Column<double>(nullable: true),
                    BottomWell = table.Column<double>(nullable: true),
                    NumBerOfPipes = table.Column<int>(nullable: true),
                    IsCoverOk = table.Column<bool>(nullable: false),
                    IsShaftOk = table.Column<bool>(nullable: false),
                    Latitude = table.Column<double>(nullable: true),
                    Longitude = table.Column<double>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WellChecks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WellCheckPhotos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Path = table.Column<string>(nullable: true),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    Latitude = table.Column<double>(nullable: false),
                    Longitude = table.Column<double>(nullable: false),
                    WellCheckId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WellCheckPhotos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WellCheckPhotos_WellChecks_WellCheckId",
                        column: x => x.WellCheckId,
                        principalTable: "WellChecks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WellCheckPhotos_WellCheckId",
                table: "WellCheckPhotos",
                column: "WellCheckId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WellCheckPhotos");

            migrationBuilder.DropTable(
                name: "WellChecks");
        }
    }
}
