using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class coordinatesoninstall : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaterielAntaller_DagsRapporterBeton_DagsRapportBetonId",
                table: "MaterielAntaller");

            migrationBuilder.DropColumn(
                name: "AntalPersoner",
                table: "TimeRegistreringerBeton");

            migrationBuilder.DropColumn(
                name: "AntalPersoner",
                table: "imeRegistreringerEkstraBeton");

            migrationBuilder.AlterColumn<int>(
                name: "DagsRapportBetonId",
                table: "MaterielAntaller",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_MaterielAntaller_DagsRapporterBeton_DagsRapportBetonId",
                table: "MaterielAntaller",
                column: "DagsRapportBetonId",
                principalTable: "DagsRapporterBeton",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaterielAntaller_DagsRapporterBeton_DagsRapportBetonId",
                table: "MaterielAntaller");

            migrationBuilder.AddColumn<int>(
                name: "AntalPersoner",
                table: "TimeRegistreringerBeton",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "DagsRapportBetonId",
                table: "MaterielAntaller",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AntalPersoner",
                table: "imeRegistreringerEkstraBeton",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_MaterielAntaller_DagsRapporterBeton_DagsRapportBetonId",
                table: "MaterielAntaller",
                column: "DagsRapportBetonId",
                principalTable: "DagsRapporterBeton",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
