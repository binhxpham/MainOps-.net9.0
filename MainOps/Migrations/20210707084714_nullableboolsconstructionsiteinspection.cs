using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class nullableboolsconstructionsiteinspection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "WasteSeperation",
                table: "ConstructionSiteInspections",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                name: "WasteDisposal",
                table: "ConstructionSiteInspections",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                name: "Walkways",
                table: "ConstructionSiteInspections",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                name: "WPA_Done",
                table: "ConstructionSiteInspections",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                name: "UnKnownContaminants",
                table: "ConstructionSiteInspections",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                name: "ToolBox_Done",
                table: "ConstructionSiteInspections",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                name: "Tidy",
                table: "ConstructionSiteInspections",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                name: "SufficientLighting",
                table: "ConstructionSiteInspections",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                name: "Sub_SafetyCond",
                table: "ConstructionSiteInspections",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                name: "Sub_MaterialStorage",
                table: "ConstructionSiteInspections",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                name: "SubContractorsUsed",
                table: "ConstructionSiteInspections",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                name: "SpecialPPE",
                table: "ConstructionSiteInspections",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                name: "SloapingAreas",
                table: "ConstructionSiteInspections",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                name: "SafetyDataSheets",
                table: "ConstructionSiteInspections",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                name: "SafetyCondition",
                table: "ConstructionSiteInspections",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                name: "SafeStorage",
                table: "ConstructionSiteInspections",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                name: "SafeAccessSite",
                table: "ConstructionSiteInspections",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                name: "ProjectManager",
                table: "ConstructionSiteInspections",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                name: "OverAllState",
                table: "ConstructionSiteInspections",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                name: "OperatingInstructions",
                table: "ConstructionSiteInspections",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                name: "OilBindingAgent",
                table: "ConstructionSiteInspections",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                name: "MeasuresPreventEnvHarm",
                table: "ConstructionSiteInspections",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                name: "MarkingDevices",
                table: "ConstructionSiteInspections",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                name: "LaddersStepsPlatforms",
                table: "ConstructionSiteInspections",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                name: "KnownContaminants",
                table: "ConstructionSiteInspections",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                name: "Hazardous_Work",
                table: "ConstructionSiteInspections",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                name: "GroundSupport",
                table: "ConstructionSiteInspections",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                name: "FloorOpenings",
                table: "ConstructionSiteInspections",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                name: "FirstAiders",
                table: "ConstructionSiteInspections",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                name: "EmergencyMeasures",
                table: "ConstructionSiteInspections",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                name: "ConditionLoadHandling",
                table: "ConstructionSiteInspections",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                name: "AllPPE",
                table: "ConstructionSiteInspections",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                name: "AdditionalInstruction",
                table: "ConstructionSiteInspections",
                nullable: true,
                oldClrType: typeof(bool));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "WasteSeperation",
                table: "ConstructionSiteInspections",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "WasteDisposal",
                table: "ConstructionSiteInspections",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Walkways",
                table: "ConstructionSiteInspections",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "WPA_Done",
                table: "ConstructionSiteInspections",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "UnKnownContaminants",
                table: "ConstructionSiteInspections",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "ToolBox_Done",
                table: "ConstructionSiteInspections",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Tidy",
                table: "ConstructionSiteInspections",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "SufficientLighting",
                table: "ConstructionSiteInspections",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Sub_SafetyCond",
                table: "ConstructionSiteInspections",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Sub_MaterialStorage",
                table: "ConstructionSiteInspections",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "SubContractorsUsed",
                table: "ConstructionSiteInspections",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "SpecialPPE",
                table: "ConstructionSiteInspections",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "SloapingAreas",
                table: "ConstructionSiteInspections",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "SafetyDataSheets",
                table: "ConstructionSiteInspections",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "SafetyCondition",
                table: "ConstructionSiteInspections",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "SafeStorage",
                table: "ConstructionSiteInspections",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "SafeAccessSite",
                table: "ConstructionSiteInspections",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "ProjectManager",
                table: "ConstructionSiteInspections",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "OverAllState",
                table: "ConstructionSiteInspections",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "OperatingInstructions",
                table: "ConstructionSiteInspections",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "OilBindingAgent",
                table: "ConstructionSiteInspections",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "MeasuresPreventEnvHarm",
                table: "ConstructionSiteInspections",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "MarkingDevices",
                table: "ConstructionSiteInspections",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "LaddersStepsPlatforms",
                table: "ConstructionSiteInspections",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "KnownContaminants",
                table: "ConstructionSiteInspections",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Hazardous_Work",
                table: "ConstructionSiteInspections",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "GroundSupport",
                table: "ConstructionSiteInspections",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "FloorOpenings",
                table: "ConstructionSiteInspections",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "FirstAiders",
                table: "ConstructionSiteInspections",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "EmergencyMeasures",
                table: "ConstructionSiteInspections",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "ConditionLoadHandling",
                table: "ConstructionSiteInspections",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "AllPPE",
                table: "ConstructionSiteInspections",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "AdditionalInstruction",
                table: "ConstructionSiteInspections",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);
        }
    }
}
