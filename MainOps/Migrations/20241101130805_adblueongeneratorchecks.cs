using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class adblueongeneratorchecks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExtraWorkBoQItems_ExtraWorkBoQs_ExtraWorkBoQId",
                table: "ExtraWorkBoQItems");

            migrationBuilder.DropIndex(
                name: "IX_ExtraWorkBoQItems_ExtraWorkBoQId",
                table: "ExtraWorkBoQItems");

            migrationBuilder.DropColumn(
                name: "ExtraWorkBoQId",
                table: "ExtraWorkBoQItems");

            migrationBuilder.AddColumn<bool>(
                name: "AdBlue_Level",
                table: "GeneratorChecks",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "AdBlue_Level_Comment",
                table: "GeneratorChecks",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GeneratorNameLocation",
                table: "GeneratorChecks",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdBlue_Level",
                table: "GeneratorChecks");

            migrationBuilder.DropColumn(
                name: "AdBlue_Level_Comment",
                table: "GeneratorChecks");

            migrationBuilder.DropColumn(
                name: "GeneratorNameLocation",
                table: "GeneratorChecks");

            migrationBuilder.AddColumn<int>(
                name: "ExtraWorkBoQId",
                table: "ExtraWorkBoQItems",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExtraWorkBoQItems_ExtraWorkBoQId",
                table: "ExtraWorkBoQItems",
                column: "ExtraWorkBoQId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExtraWorkBoQItems_ExtraWorkBoQs_ExtraWorkBoQId",
                table: "ExtraWorkBoQItems",
                column: "ExtraWorkBoQId",
                principalTable: "ExtraWorkBoQs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
