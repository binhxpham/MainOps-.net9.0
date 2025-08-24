using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class pumptesting2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Pressure",
                table: "PumptestingDatas",
                nullable: true,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<double>(
                name: "Flow",
                table: "PumptestingDatas",
                nullable: true,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<int>(
                name: "Duration",
                table: "PumptestingDatas",
                nullable: true,
                oldClrType: typeof(TimeSpan));

            migrationBuilder.CreateTable(
                name: "PumptestingPhoto",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    path = table.Column<string>(nullable: true),
                    PumptestingId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PumptestingPhoto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PumptestingPhoto_PumpTestings_PumptestingId",
                        column: x => x.PumptestingId,
                        principalTable: "PumpTestings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PumptestingPhoto_PumptestingId",
                table: "PumptestingPhoto",
                column: "PumptestingId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PumptestingPhoto");

            migrationBuilder.AlterColumn<double>(
                name: "Pressure",
                table: "PumptestingDatas",
                nullable: false,
                oldClrType: typeof(double),
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Flow",
                table: "PumptestingDatas",
                nullable: false,
                oldClrType: typeof(double),
                oldNullable: true);

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "Duration",
                table: "PumptestingDatas",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
