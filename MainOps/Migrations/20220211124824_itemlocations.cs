using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class itemlocations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Internal_Rent",
                table: "HJItemClasses",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "Item_Locations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    HJItemId = table.Column<int>(nullable: false),
                    ProjectId = table.Column<int>(nullable: false),
                    SubProjectId = table.Column<int>(nullable: true),
                    StartTime = table.Column<DateTime>(nullable: false),
                    EndTime = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Item_Locations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Item_Locations_HJItems_HJItemId",
                        column: x => x.HJItemId,
                        principalTable: "HJItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Item_Locations_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Item_Locations_SubProjects_SubProjectId",
                        column: x => x.SubProjectId,
                        principalTable: "SubProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Item_Locations_HJItemId",
                table: "Item_Locations",
                column: "HJItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Item_Locations_ProjectId",
                table: "Item_Locations",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Item_Locations_SubProjectId",
                table: "Item_Locations",
                column: "SubProjectId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Item_Locations");

            migrationBuilder.DropColumn(
                name: "Internal_Rent",
                table: "HJItemClasses");
        }
    }
}
