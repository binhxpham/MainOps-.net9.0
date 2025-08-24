using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class sandandcastingtypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SandType",
                table: "SandWellLayers");

            migrationBuilder.AddColumn<int>(
                name: "SandTypeId",
                table: "SandWellLayers",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CastingTypeId",
                table: "BentoniteWellLayers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CastingTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TypeOfCasting = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CastingTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SandTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TýpeOfSand = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SandTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SandWellLayers_SandTypeId",
                table: "SandWellLayers",
                column: "SandTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BentoniteWellLayers_CastingTypeId",
                table: "BentoniteWellLayers",
                column: "CastingTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_BentoniteWellLayers_CastingTypes_CastingTypeId",
                table: "BentoniteWellLayers",
                column: "CastingTypeId",
                principalTable: "CastingTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SandWellLayers_SandTypes_SandTypeId",
                table: "SandWellLayers",
                column: "SandTypeId",
                principalTable: "SandTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BentoniteWellLayers_CastingTypes_CastingTypeId",
                table: "BentoniteWellLayers");

            migrationBuilder.DropForeignKey(
                name: "FK_SandWellLayers_SandTypes_SandTypeId",
                table: "SandWellLayers");

            migrationBuilder.DropTable(
                name: "CastingTypes");

            migrationBuilder.DropTable(
                name: "SandTypes");

            migrationBuilder.DropIndex(
                name: "IX_SandWellLayers_SandTypeId",
                table: "SandWellLayers");

            migrationBuilder.DropIndex(
                name: "IX_BentoniteWellLayers_CastingTypeId",
                table: "BentoniteWellLayers");

            migrationBuilder.DropColumn(
                name: "SandTypeId",
                table: "SandWellLayers");

            migrationBuilder.DropColumn(
                name: "CastingTypeId",
                table: "BentoniteWellLayers");

            migrationBuilder.AddColumn<string>(
                name: "SandType",
                table: "SandWellLayers",
                nullable: true);
        }
    }
}
