using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class removingforeignkeys : Migration
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

            migrationBuilder.DropIndex(
                name: "IX_Filter2Layers_LayerTypeOverId",
                table: "Filter2Layers");

            migrationBuilder.DropIndex(
                name: "IX_Filter2Layers_LayerTypeUnderId",
                table: "Filter2Layers");

            migrationBuilder.DropIndex(
                name: "IX_Filter1Layers_LayerTypeOverId",
                table: "Filter1Layers");

            migrationBuilder.DropIndex(
                name: "IX_Filter1Layers_LayerTypeUnderId",
                table: "Filter1Layers");

            migrationBuilder.DropIndex(
                name: "IX_BentoniteLayers_LayerTypeOverId",
                table: "BentoniteLayers");

            migrationBuilder.DropIndex(
                name: "IX_BentoniteLayers_LayerTypeUnderId",
                table: "BentoniteLayers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Filter2Layers_LayerTypeOverId",
                table: "Filter2Layers",
                column: "LayerTypeOverId");

            migrationBuilder.CreateIndex(
                name: "IX_Filter2Layers_LayerTypeUnderId",
                table: "Filter2Layers",
                column: "LayerTypeUnderId");

            migrationBuilder.CreateIndex(
                name: "IX_Filter1Layers_LayerTypeOverId",
                table: "Filter1Layers",
                column: "LayerTypeOverId");

            migrationBuilder.CreateIndex(
                name: "IX_Filter1Layers_LayerTypeUnderId",
                table: "Filter1Layers",
                column: "LayerTypeUnderId");

            migrationBuilder.CreateIndex(
                name: "IX_BentoniteLayers_LayerTypeOverId",
                table: "BentoniteLayers",
                column: "LayerTypeOverId");

            migrationBuilder.CreateIndex(
                name: "IX_BentoniteLayers_LayerTypeUnderId",
                table: "BentoniteLayers",
                column: "LayerTypeUnderId");

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
        }
    }
}
