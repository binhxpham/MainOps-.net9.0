using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class testingifsomethingismissing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropForeignKey(
                name: "FK_WellLayers_Layers_LayerId",
                table: "WellLayers");


            migrationBuilder.AlterColumn<int>(
                name: "LayerId",
                table: "WellLayers",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_WellLayers_Layers_LayerId",
                table: "WellLayers",
                column: "LayerId",
                principalTable: "Layers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WellLayers_Layers_LayerId",
                table: "WellLayers");

            migrationBuilder.AlterColumn<int>(
                name: "LayerId",
                table: "WellLayers",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WellId",
                table: "Layers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Layers_WellId",
                table: "Layers",
                column: "WellId");

            migrationBuilder.AddForeignKey(
                name: "FK_Layers_Wells_WellId",
                table: "Layers",
                column: "WellId",
                principalTable: "Wells",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WellLayers_Layers_LayerId",
                table: "WellLayers",
                column: "LayerId",
                principalTable: "Layers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
