using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class nonnullableprojectid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_DagsRapporterBeton_Projects_ProjectId",
            //    table: "DagsRapporterBeton");

            //migrationBuilder.AlterColumn<int>(
            //    name: "ProjectId",
            //    table: "DagsRapporterBeton",
            //    nullable: false,
            //    oldClrType: typeof(int),
            //    oldNullable: true);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_DagsRapporterBeton_Projects_ProjectId",
            //    table: "DagsRapporterBeton",
            //    column: "ProjectId",
            //    principalTable: "Projects",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DagsRapporterBeton_Projects_ProjectId",
                table: "DagsRapporterBeton");

            migrationBuilder.AlterColumn<int>(
                name: "ProjectId",
                table: "DagsRapporterBeton",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_DagsRapporterBeton_Projects_ProjectId",
                table: "DagsRapporterBeton",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
