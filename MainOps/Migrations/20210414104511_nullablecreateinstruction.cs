using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class nullablecreateinstruction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BentoniteLayers_LayerType_LayerTypeOverId",
                table: "BentoniteLayers");

            migrationBuilder.DropForeignKey(
                name: "FK_BentoniteLayers_LayerType_LayerTypeUnderId",
                table: "BentoniteLayers");

            migrationBuilder.DropForeignKey(
                name: "FK_Filter1Layers_LayerType_LayerTypeOverId",
                table: "Filter1Layers");

            migrationBuilder.DropForeignKey(
                name: "FK_Filter1Layers_LayerType_LayerTypeUnderId",
                table: "Filter1Layers");

            migrationBuilder.DropForeignKey(
                name: "FK_Filter2Layers_LayerType_LayerTypeOverId",
                table: "Filter2Layers");

            migrationBuilder.DropForeignKey(
                name: "FK_Filter2Layers_LayerType_LayerTypeUnderId",
                table: "Filter2Layers");

            migrationBuilder.DropForeignKey(
                name: "FK_LayerType_Layers_LayerId",
                table: "LayerType");

            migrationBuilder.AlterColumn<int>(
                name: "LayerId",
                table: "LayerType",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "LayerTypeUnderId",
                table: "Filter2Layers",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "LayerTypeOverId",
                table: "Filter2Layers",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "LayerTypeUnderId",
                table: "Filter1Layers",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "LayerTypeOverId",
                table: "Filter1Layers",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "LayerTypeUnderId",
                table: "BentoniteLayers",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "LayerTypeOverId",
                table: "BentoniteLayers",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_BentoniteLayers_LayerType_LayerTypeOverId",
                table: "BentoniteLayers",
                column: "LayerTypeOverId",
                principalTable: "LayerType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BentoniteLayers_LayerType_LayerTypeUnderId",
                table: "BentoniteLayers",
                column: "LayerTypeUnderId",
                principalTable: "LayerType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Filter1Layers_LayerType_LayerTypeOverId",
                table: "Filter1Layers",
                column: "LayerTypeOverId",
                principalTable: "LayerType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Filter1Layers_LayerType_LayerTypeUnderId",
                table: "Filter1Layers",
                column: "LayerTypeUnderId",
                principalTable: "LayerType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Filter2Layers_LayerType_LayerTypeOverId",
                table: "Filter2Layers",
                column: "LayerTypeOverId",
                principalTable: "LayerType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Filter2Layers_LayerType_LayerTypeUnderId",
                table: "Filter2Layers",
                column: "LayerTypeUnderId",
                principalTable: "LayerType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LayerType_Layers_LayerId",
                table: "LayerType",
                column: "LayerId",
                principalTable: "Layers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BentoniteLayers_LayerType_LayerTypeOverId",
                table: "BentoniteLayers");

            migrationBuilder.DropForeignKey(
                name: "FK_BentoniteLayers_LayerType_LayerTypeUnderId",
                table: "BentoniteLayers");

            migrationBuilder.DropForeignKey(
                name: "FK_Filter1Layers_LayerType_LayerTypeOverId",
                table: "Filter1Layers");

            migrationBuilder.DropForeignKey(
                name: "FK_Filter1Layers_LayerType_LayerTypeUnderId",
                table: "Filter1Layers");

            migrationBuilder.DropForeignKey(
                name: "FK_Filter2Layers_LayerType_LayerTypeOverId",
                table: "Filter2Layers");

            migrationBuilder.DropForeignKey(
                name: "FK_Filter2Layers_LayerType_LayerTypeUnderId",
                table: "Filter2Layers");

            migrationBuilder.DropForeignKey(
                name: "FK_LayerType_Layers_LayerId",
                table: "LayerType");

            migrationBuilder.AlterColumn<int>(
                name: "LayerId",
                table: "LayerType",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "LayerTypeUnderId",
                table: "Filter2Layers",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "LayerTypeOverId",
                table: "Filter2Layers",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "LayerTypeUnderId",
                table: "Filter1Layers",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "LayerTypeOverId",
                table: "Filter1Layers",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "LayerTypeUnderId",
                table: "BentoniteLayers",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "LayerTypeOverId",
                table: "BentoniteLayers",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BentoniteLayers_LayerType_LayerTypeOverId",
                table: "BentoniteLayers",
                column: "LayerTypeOverId",
                principalTable: "LayerType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BentoniteLayers_LayerType_LayerTypeUnderId",
                table: "BentoniteLayers",
                column: "LayerTypeUnderId",
                principalTable: "LayerType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Filter1Layers_LayerType_LayerTypeOverId",
                table: "Filter1Layers",
                column: "LayerTypeOverId",
                principalTable: "LayerType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Filter1Layers_LayerType_LayerTypeUnderId",
                table: "Filter1Layers",
                column: "LayerTypeUnderId",
                principalTable: "LayerType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Filter2Layers_LayerType_LayerTypeOverId",
                table: "Filter2Layers",
                column: "LayerTypeOverId",
                principalTable: "LayerType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Filter2Layers_LayerType_LayerTypeUnderId",
                table: "Filter2Layers",
                column: "LayerTypeUnderId",
                principalTable: "LayerType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LayerType_Layers_LayerId",
                table: "LayerType",
                column: "LayerId",
                principalTable: "Layers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
