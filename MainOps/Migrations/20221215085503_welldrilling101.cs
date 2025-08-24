using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class welldrilling101 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pipe_1_Description",
                table: "Wells");

            migrationBuilder.DropColumn(
                name: "Pipe_2_Description",
                table: "Wells");

            migrationBuilder.AddColumn<string>(
                name: "SoilColor",
                table: "SoilSamples",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BentoniteWellLayers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    WellId = table.Column<int>(nullable: true),
                    meter_start = table.Column<double>(nullable: false),
                    meter_end = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BentoniteWellLayers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BentoniteWellLayers_Wells_WellId",
                        column: x => x.WellId,
                        principalTable: "Wells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FilterWellLayers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    WellId = table.Column<int>(nullable: true),
                    meter_start = table.Column<double>(nullable: false),
                    meter_end = table.Column<double>(nullable: false),
                    Slitsize = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilterWellLayers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FilterWellLayers_Wells_WellId",
                        column: x => x.WellId,
                        principalTable: "Wells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SandWellLayers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    WellId = table.Column<int>(nullable: true),
                    meter_start = table.Column<double>(nullable: false),
                    meter_end = table.Column<double>(nullable: false),
                    SandType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SandWellLayers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SandWellLayers_Wells_WellId",
                        column: x => x.WellId,
                        principalTable: "Wells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BentoniteWellLayers_WellId",
                table: "BentoniteWellLayers",
                column: "WellId");

            migrationBuilder.CreateIndex(
                name: "IX_FilterWellLayers_WellId",
                table: "FilterWellLayers",
                column: "WellId");

            migrationBuilder.CreateIndex(
                name: "IX_SandWellLayers_WellId",
                table: "SandWellLayers",
                column: "WellId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BentoniteWellLayers");

            migrationBuilder.DropTable(
                name: "FilterWellLayers");

            migrationBuilder.DropTable(
                name: "SandWellLayers");

            migrationBuilder.DropColumn(
                name: "SoilColor",
                table: "SoilSamples");

            migrationBuilder.AddColumn<string>(
                name: "Pipe_1_Description",
                table: "Wells",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pipe_2_Description",
                table: "Wells",
                nullable: true);
        }
    }
}
