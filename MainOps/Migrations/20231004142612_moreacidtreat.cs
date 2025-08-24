using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class moreacidtreat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Coments",
                table: "AcidTreatments",
                newName: "Comments");

            migrationBuilder.AddColumn<double>(
                name: "Acid_Meter_After",
                table: "AcidTreatments",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Acid_Meter_Before",
                table: "AcidTreatments",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Ref_Level",
                table: "AcidTreatments",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "TimeStamp",
                table: "AcidDatas",
                nullable: false,
                oldClrType: typeof(TimeSpan));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Acid_Meter_After",
                table: "AcidTreatments");

            migrationBuilder.DropColumn(
                name: "Acid_Meter_Before",
                table: "AcidTreatments");

            migrationBuilder.DropColumn(
                name: "Ref_Level",
                table: "AcidTreatments");

            migrationBuilder.RenameColumn(
                name: "Comments",
                table: "AcidTreatments",
                newName: "Coments");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "TimeStamp",
                table: "AcidDatas",
                nullable: false,
                oldClrType: typeof(DateTime));
        }
    }
}
