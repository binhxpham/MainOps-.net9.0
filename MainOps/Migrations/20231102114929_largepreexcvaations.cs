using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class largepreexcvaations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Airlifts_Projects_ProjectId",
                table: "Airlifts");

            migrationBuilder.DropColumn(
                name: "WellName",
                table: "Airlifts");

            migrationBuilder.AddColumn<bool>(
                name: "Large",
                table: "PreExcavations",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "ProjectId",
                table: "Airlifts",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_Airlifts_Projects_ProjectId",
                table: "Airlifts",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Airlifts_Projects_ProjectId",
                table: "Airlifts");

            migrationBuilder.DropColumn(
                name: "Large",
                table: "PreExcavations");

            migrationBuilder.AlterColumn<int>(
                name: "ProjectId",
                table: "Airlifts",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WellName",
                table: "Airlifts",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Airlifts_Projects_ProjectId",
                table: "Airlifts",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
