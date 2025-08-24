using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class agreementnumbers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AgreementDate",
                table: "ItemTypes",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AgreementNumber",
                table: "ItemTypes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AgreementDate",
                table: "ItemTypes");

            migrationBuilder.DropColumn(
                name: "AgreementNumber",
                table: "ItemTypes");
        }
    }
}
