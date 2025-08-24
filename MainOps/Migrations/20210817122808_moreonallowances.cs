using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class moreonallowances : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CalculationOfDietsAndSmallNecessitiesSubTotal",
                table: "Diets",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DeductionBreakFastSubTotal",
                table: "Diets",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DeductionDinnerSubTotal",
                table: "Diets",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DeductionLunchSubTotal",
                table: "Diets",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "HourAddonSubTotal",
                table: "Diets",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "LivingInCamperWagonSubTotal",
                table: "Diets",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SelfContainedExpensesSubTotal",
                table: "Diets",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Total",
                table: "Diets",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CalculationOfDietsAndSmallNecessitiesSubTotal",
                table: "Diets");

            migrationBuilder.DropColumn(
                name: "DeductionBreakFastSubTotal",
                table: "Diets");

            migrationBuilder.DropColumn(
                name: "DeductionDinnerSubTotal",
                table: "Diets");

            migrationBuilder.DropColumn(
                name: "DeductionLunchSubTotal",
                table: "Diets");

            migrationBuilder.DropColumn(
                name: "HourAddonSubTotal",
                table: "Diets");

            migrationBuilder.DropColumn(
                name: "LivingInCamperWagonSubTotal",
                table: "Diets");

            migrationBuilder.DropColumn(
                name: "SelfContainedExpensesSubTotal",
                table: "Diets");

            migrationBuilder.DropColumn(
                name: "Total",
                table: "Diets");
        }
    }
}
