using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class preexcavations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PreExcavations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ProjectId = table.Column<int>(nullable: false),
                    SubProjectId = table.Column<int>(nullable: true),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    MeasPointId = table.Column<int>(nullable: true),
                    wellname = table.Column<string>(nullable: true),
                    CablesFound = table.Column<bool>(nullable: false),
                    Comments = table.Column<string>(nullable: true),
                    DoneBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreExcavations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PreExcavations_MeasPoints_MeasPointId",
                        column: x => x.MeasPointId,
                        principalTable: "MeasPoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PreExcavations_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PreExcavations_SubProjects_SubProjectId",
                        column: x => x.SubProjectId,
                        principalTable: "SubProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PreExcavationAfterPhotos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    path = table.Column<string>(nullable: true),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    PreExcavationId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreExcavationAfterPhotos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PreExcavationAfterPhotos_PreExcavations_PreExcavationId",
                        column: x => x.PreExcavationId,
                        principalTable: "PreExcavations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PreExcavationBeforePhotos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    path = table.Column<string>(nullable: true),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    PreExcavationId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreExcavationBeforePhotos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PreExcavationBeforePhotos_PreExcavations_PreExcavationId",
                        column: x => x.PreExcavationId,
                        principalTable: "PreExcavations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PreExcavationPhotos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    path = table.Column<string>(nullable: true),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    PreExcavationId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreExcavationPhotos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PreExcavationPhotos_PreExcavations_PreExcavationId",
                        column: x => x.PreExcavationId,
                        principalTable: "PreExcavations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PreExcavationAfterPhotos_PreExcavationId",
                table: "PreExcavationAfterPhotos",
                column: "PreExcavationId");

            migrationBuilder.CreateIndex(
                name: "IX_PreExcavationBeforePhotos_PreExcavationId",
                table: "PreExcavationBeforePhotos",
                column: "PreExcavationId");

            migrationBuilder.CreateIndex(
                name: "IX_PreExcavationPhotos_PreExcavationId",
                table: "PreExcavationPhotos",
                column: "PreExcavationId");

            migrationBuilder.CreateIndex(
                name: "IX_PreExcavations_MeasPointId",
                table: "PreExcavations",
                column: "MeasPointId");

            migrationBuilder.CreateIndex(
                name: "IX_PreExcavations_ProjectId",
                table: "PreExcavations",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_PreExcavations_SubProjectId",
                table: "PreExcavations",
                column: "SubProjectId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PreExcavationAfterPhotos");

            migrationBuilder.DropTable(
                name: "PreExcavationBeforePhotos");

            migrationBuilder.DropTable(
                name: "PreExcavationPhotos");

            migrationBuilder.DropTable(
                name: "PreExcavations");
        }
    }
}
