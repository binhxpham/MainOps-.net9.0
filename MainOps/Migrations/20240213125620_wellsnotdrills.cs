using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class wellsnotdrills : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WaterHandlings_Drillings_DrillId",
                table: "WaterHandlings");

            migrationBuilder.RenameColumn(
                name: "DrillId",
                table: "WaterHandlings",
                newName: "WellId");

            migrationBuilder.RenameIndex(
                name: "IX_WaterHandlings_DrillId",
                table: "WaterHandlings",
                newName: "IX_WaterHandlings_WellId");

            migrationBuilder.AddColumn<bool>(
                name: "DischargeAvailable",
                table: "ClearPumpTests",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_WaterHandlings_Wells_WellId",
                table: "WaterHandlings",
                column: "WellId",
                principalTable: "Wells",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WaterHandlings_Wells_WellId",
                table: "WaterHandlings");

            migrationBuilder.DropColumn(
                name: "DischargeAvailable",
                table: "ClearPumpTests");

            migrationBuilder.RenameColumn(
                name: "WellId",
                table: "WaterHandlings",
                newName: "DrillId");

            migrationBuilder.RenameIndex(
                name: "IX_WaterHandlings_WellId",
                table: "WaterHandlings",
                newName: "IX_WaterHandlings_DrillId");

            migrationBuilder.AddForeignKey(
                name: "FK_WaterHandlings_Drillings_DrillId",
                table: "WaterHandlings",
                column: "DrillId",
                principalTable: "Drillings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
