using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class grouting2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Groutings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ProjectId = table.Column<int>(nullable: false),
                    StartTime = table.Column<DateTime>(nullable: false),
                    EndTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groutings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Groutings_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroutAfterPhotos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    path = table.Column<string>(nullable: true),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    GroutingId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroutAfterPhotos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroutAfterPhotos_Groutings_GroutingId",
                        column: x => x.GroutingId,
                        principalTable: "Groutings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GroutBeforePhotos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    path = table.Column<string>(nullable: true),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    GroutingId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroutBeforePhotos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroutBeforePhotos_Groutings_GroutingId",
                        column: x => x.GroutingId,
                        principalTable: "Groutings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GroutDataDevice",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    GroutingId = table.Column<int>(nullable: true),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    FlowData = table.Column<double>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroutDataDevice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroutDataDevice_Groutings_GroutingId",
                        column: x => x.GroutingId,
                        principalTable: "Groutings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GroutGroutPhotos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    path = table.Column<string>(nullable: true),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    GroutingId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroutGroutPhotos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroutGroutPhotos_Groutings_GroutingId",
                        column: x => x.GroutingId,
                        principalTable: "Groutings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroutAfterPhotos_GroutingId",
                table: "GroutAfterPhotos",
                column: "GroutingId");

            migrationBuilder.CreateIndex(
                name: "IX_GroutBeforePhotos_GroutingId",
                table: "GroutBeforePhotos",
                column: "GroutingId");

            migrationBuilder.CreateIndex(
                name: "IX_GroutDataDevice_GroutingId",
                table: "GroutDataDevice",
                column: "GroutingId");

            migrationBuilder.CreateIndex(
                name: "IX_GroutGroutPhotos_GroutingId",
                table: "GroutGroutPhotos",
                column: "GroutingId");

            migrationBuilder.CreateIndex(
                name: "IX_Groutings_ProjectId",
                table: "Groutings",
                column: "ProjectId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroutAfterPhotos");

            migrationBuilder.DropTable(
                name: "GroutBeforePhotos");

            migrationBuilder.DropTable(
                name: "GroutDataDevice");

            migrationBuilder.DropTable(
                name: "GroutGroutPhotos");

            migrationBuilder.DropTable(
                name: "Groutings");
        }
    }
}
