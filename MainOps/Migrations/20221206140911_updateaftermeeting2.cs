using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class updateaftermeeting2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Item_Locations_HJItems_HJItemId",
                table: "Item_Locations");

            migrationBuilder.DropColumn(
                name: "Status_No",
                table: "StatusDescriptions");

            migrationBuilder.DropColumn(
                name: "DrillContractValue",
                table: "ProjectStatuses");

            migrationBuilder.DropColumn(
                name: "ReturnedOnTime",
                table: "ProjectStatuses");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "ProjectCategories");

            migrationBuilder.RenameColumn(
                name: "TenderResponisble",
                table: "ProjectStatuses",
                newName: "TenderManager");

            migrationBuilder.RenameColumn(
                name: "Technique",
                table: "ProjectStatuses",
                newName: "ProjectName");

            migrationBuilder.RenameColumn(
                name: "RequestedReturnDate",
                table: "ProjectStatuses",
                newName: "TenderDate");

            migrationBuilder.RenameColumn(
                name: "InstallationValue",
                table: "ProjectStatuses",
                newName: "OptionsValue");

            migrationBuilder.RenameColumn(
                name: "DateReceived",
                table: "ProjectStatuses",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "DateGiven",
                table: "ProjectStatuses",
                newName: "DateSubmitted");

            migrationBuilder.RenameColumn(
                name: "Currency",
                table: "ProjectStatuses",
                newName: "ProjectDescription");

            migrationBuilder.RenameColumn(
                name: "ContractName",
                table: "ProjectStatuses",
                newName: "Client");

            migrationBuilder.AddColumn<DateTime>(
                name: "ClientTenderDate",
                table: "ProjectStatuses",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "ProjectStatuses",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "WinChance",
                table: "ProjectStatuses",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<int>(
                name: "HJItemId",
                table: "Item_Locations",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_Item_Locations_HJItems_HJItemId",
                table: "Item_Locations",
                column: "HJItemId",
                principalTable: "HJItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Item_Locations_HJItems_HJItemId",
                table: "Item_Locations");

            migrationBuilder.DropColumn(
                name: "ClientTenderDate",
                table: "ProjectStatuses");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "ProjectStatuses");

            migrationBuilder.DropColumn(
                name: "WinChance",
                table: "ProjectStatuses");

            migrationBuilder.RenameColumn(
                name: "TenderManager",
                table: "ProjectStatuses",
                newName: "TenderResponisble");

            migrationBuilder.RenameColumn(
                name: "TenderDate",
                table: "ProjectStatuses",
                newName: "RequestedReturnDate");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "ProjectStatuses",
                newName: "DateReceived");

            migrationBuilder.RenameColumn(
                name: "ProjectName",
                table: "ProjectStatuses",
                newName: "Technique");

            migrationBuilder.RenameColumn(
                name: "ProjectDescription",
                table: "ProjectStatuses",
                newName: "Currency");

            migrationBuilder.RenameColumn(
                name: "OptionsValue",
                table: "ProjectStatuses",
                newName: "InstallationValue");

            migrationBuilder.RenameColumn(
                name: "DateSubmitted",
                table: "ProjectStatuses",
                newName: "DateGiven");

            migrationBuilder.RenameColumn(
                name: "Client",
                table: "ProjectStatuses",
                newName: "ContractName");

            migrationBuilder.AddColumn<int>(
                name: "Status_No",
                table: "StatusDescriptions",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "DrillContractValue",
                table: "ProjectStatuses",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "ReturnedOnTime",
                table: "ProjectStatuses",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "ProjectCategories",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "HJItemId",
                table: "Item_Locations",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Item_Locations_HJItems_HJItemId",
                table: "Item_Locations",
                column: "HJItemId",
                principalTable: "HJItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
