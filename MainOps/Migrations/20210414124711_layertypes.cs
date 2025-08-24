using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class layertypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LayerType_Layers_LayerId",
                table: "LayerType");

            migrationBuilder.DropForeignKey(
                name: "FK_LayerType_WellDrillingInstructions_WellDrillingInstructionId",
                table: "LayerType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LayerType",
                table: "LayerType");

            migrationBuilder.RenameTable(
                name: "LayerType",
                newName: "LayerTypes");

            migrationBuilder.RenameIndex(
                name: "IX_LayerType_WellDrillingInstructionId",
                table: "LayerTypes",
                newName: "IX_LayerTypes_WellDrillingInstructionId");

            migrationBuilder.RenameIndex(
                name: "IX_LayerType_LayerId",
                table: "LayerTypes",
                newName: "IX_LayerTypes_LayerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LayerTypes",
                table: "LayerTypes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LayerTypes_Layers_LayerId",
                table: "LayerTypes",
                column: "LayerId",
                principalTable: "Layers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LayerTypes_WellDrillingInstructions_WellDrillingInstructionId",
                table: "LayerTypes",
                column: "WellDrillingInstructionId",
                principalTable: "WellDrillingInstructions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LayerTypes_Layers_LayerId",
                table: "LayerTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_LayerTypes_WellDrillingInstructions_WellDrillingInstructionId",
                table: "LayerTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LayerTypes",
                table: "LayerTypes");

            migrationBuilder.RenameTable(
                name: "LayerTypes",
                newName: "LayerType");

            migrationBuilder.RenameIndex(
                name: "IX_LayerTypes_WellDrillingInstructionId",
                table: "LayerType",
                newName: "IX_LayerType_WellDrillingInstructionId");

            migrationBuilder.RenameIndex(
                name: "IX_LayerTypes_LayerId",
                table: "LayerType",
                newName: "IX_LayerType_LayerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LayerType",
                table: "LayerType",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LayerType_Layers_LayerId",
                table: "LayerType",
                column: "LayerId",
                principalTable: "Layers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LayerType_WellDrillingInstructions_WellDrillingInstructionId",
                table: "LayerType",
                column: "WellDrillingInstructionId",
                principalTable: "WellDrillingInstructions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
