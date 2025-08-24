using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class morestufffordrill : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DoneBy",
                table: "Drillings",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "Drillings",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SubProjectId",
                table: "Drillings",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeStamp",
                table: "Drillings",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Drillings_ProjectId",
                table: "Drillings",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Drillings_SubProjectId",
                table: "Drillings",
                column: "SubProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Drillings_Projects_ProjectId",
                table: "Drillings",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Drillings_SubProjects_SubProjectId",
                table: "Drillings",
                column: "SubProjectId",
                principalTable: "SubProjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Drillings_Projects_ProjectId",
                table: "Drillings");

            migrationBuilder.DropForeignKey(
                name: "FK_Drillings_SubProjects_SubProjectId",
                table: "Drillings");

            migrationBuilder.DropIndex(
                name: "IX_Drillings_ProjectId",
                table: "Drillings");

            migrationBuilder.DropIndex(
                name: "IX_Drillings_SubProjectId",
                table: "Drillings");

            migrationBuilder.DropColumn(
                name: "DoneBy",
                table: "Drillings");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "Drillings");

            migrationBuilder.DropColumn(
                name: "SubProjectId",
                table: "Drillings");

            migrationBuilder.DropColumn(
                name: "TimeStamp",
                table: "Drillings");
        }
    }
}
