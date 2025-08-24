using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class itemactivities1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ItemActivities",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TheDate = table.Column<DateTime>(nullable: false),
                    WasActive = table.Column<bool>(nullable: false),
                    HJItemId = table.Column<int>(nullable: true),
                    UniqueID = table.Column<string>(nullable: true),
                    ProjectId = table.Column<int>(nullable: true),
                    SubProjectId = table.Column<int>(nullable: true),
                    Latitude = table.Column<double>(nullable: true),
                    Longitude = table.Column<double>(nullable: true),
                    ItemName = table.Column<string>(nullable: true),
                    InstallId = table.Column<int>(nullable: true),
                    ArrivalId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemActivities_Arrivals_ArrivalId",
                        column: x => x.ArrivalId,
                        principalTable: "Arrivals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemActivities_HJItems_HJItemId",
                        column: x => x.HJItemId,
                        principalTable: "HJItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemActivities_Installations_InstallId",
                        column: x => x.InstallId,
                        principalTable: "Installations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemActivities_ArrivalId",
                table: "ItemActivities",
                column: "ArrivalId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemActivities_HJItemId",
                table: "ItemActivities",
                column: "HJItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemActivities_InstallId",
                table: "ItemActivities",
                column: "InstallId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemActivities");
        }
    }
}
