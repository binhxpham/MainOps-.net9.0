using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class layersupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "LayerId",
                table: "WellLayers",
                nullable: true,
                defaultValue: null);
            //migrationBuilder.AddColumn<int>(
            //    name: "LayerId",
            //    table: "WellLayers",
            //    nullable: true,
            //    defaultValue: null);


            //migrationBuilder.CreateIndex(
            //    name: "IX_WellLayers_LayerId",
            //    table: "WellLayers",
            //    column: "LayerId");



            migrationBuilder.AddForeignKey(
                name: "FK_WellLayers_Layers_LayerId",
                table: "WellLayers",
                column: "LayerId",
                principalTable: "Layers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Layers_Wells_WellId",
                table: "Layers");

            migrationBuilder.DropForeignKey(
                name: "FK_WellLayers_Layers_LayerId",
                table: "WellLayers");

            migrationBuilder.DropIndex(
                name: "IX_WellLayers_LayerId",
                table: "WellLayers");

            migrationBuilder.DropIndex(
                name: "IX_Layers_WellId",
                table: "Layers");

            migrationBuilder.DropColumn(
                name: "LayerId",
                table: "WellLayers");

            migrationBuilder.DropColumn(
                name: "WellId",
                table: "Layers");

            migrationBuilder.AddColumn<string>(
                name: "Ground_Type",
                table: "WellLayers",
                nullable: true);
        }
    }
}
