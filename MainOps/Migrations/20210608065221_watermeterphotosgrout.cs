using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class watermeterphotosgrout : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           
            migrationBuilder.CreateTable(
                name: "GroutWMAfterPhotos",
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
                    table.PrimaryKey("PK_GroutWMAfterPhotos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroutWMAfterPhotos_Groutings_GroutingId",
                        column: x => x.GroutingId,
                        principalTable: "Groutings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GroutWMBeforePhotos",
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
                    table.PrimaryKey("PK_GroutWMBeforePhotos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroutWMBeforePhotos_Groutings_GroutingId",
                        column: x => x.GroutingId,
                        principalTable: "Groutings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroutWMAfterPhotos_GroutingId",
                table: "GroutWMAfterPhotos",
                column: "GroutingId");

            migrationBuilder.CreateIndex(
                name: "IX_GroutWMBeforePhotos_GroutingId",
                table: "GroutWMBeforePhotos",
                column: "GroutingId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroutWMAfterPhotos");

            migrationBuilder.DropTable(
                name: "GroutWMBeforePhotos");

            migrationBuilder.AlterColumn<string>(
                name: "Work_Performed",
                table: "Daily_Report_2s",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 10000);
        }
    }
}
