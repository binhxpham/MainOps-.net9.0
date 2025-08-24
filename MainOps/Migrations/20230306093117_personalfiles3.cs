using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class personalfiles3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<string>(
            //    name: "ApplicationUserId",
            //    table: "PersonalFiles",
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "UserId",
            //    table: "PersonalFiles",
            //    nullable: true);

            //migrationBuilder.CreateIndex(
            //    name: "IX_PersonalFiles_ApplicationUserId",
            //    table: "PersonalFiles",
            //    column: "ApplicationUserId");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_PersonalFiles_AspNetUsers_ApplicationUserId",
            //    table: "PersonalFiles",
            //    column: "ApplicationUserId",
            //    principalTable: "AspNetUsers",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PersonalFiles_AspNetUsers_ApplicationUserId",
                table: "PersonalFiles");

            migrationBuilder.DropIndex(
                name: "IX_PersonalFiles_ApplicationUserId",
                table: "PersonalFiles");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "PersonalFiles");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "PersonalFiles");
        }
    }
}
