using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class checkitout : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HourRegistrations_Divisions_DivisionId",
                table: "HourRegistrations");

            migrationBuilder.DropForeignKey(
                name: "FK_HourRegistrations_Ongoing_Divisions_DivisionId",
                table: "HourRegistrations_Ongoing");

            migrationBuilder.DropIndex(
                name: "IX_HourRegistrations_Ongoing_DivisionId",
                table: "HourRegistrations_Ongoing");

            migrationBuilder.DropIndex(
                name: "IX_HourRegistrations_DivisionId",
                table: "HourRegistrations");


            migrationBuilder.AlterColumn<int>(
                name: "DivisionId",
                table: "HourRegistrations_Ongoing",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DivisionId",
                table: "HourRegistrations",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_HourRegistrations_Divisions_DivisionId",
                table: "HourRegistrations",
                column: "DivisionId",
                principalTable: "Divisions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HourRegistrations_Ongoing_Divisions_DivisionId",
                table: "HourRegistrations_Ongoing",
                column: "DivisionId",
                principalTable: "Divisions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
