using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class photodoc3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PhotoDoc_PhotoDocumenations_PhotoDocumenationId",
                table: "PhotoDoc");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PhotoDoc",
                table: "PhotoDoc");

            migrationBuilder.RenameTable(
                name: "PhotoDoc",
                newName: "PhotoDocs");

            migrationBuilder.RenameColumn(
                name: "PhotoDocumenationId",
                table: "PhotoDocs",
                newName: "PhotoDocumentationId");

            migrationBuilder.RenameIndex(
                name: "IX_PhotoDoc_PhotoDocumenationId",
                table: "PhotoDocs",
                newName: "IX_PhotoDocs_PhotoDocumentationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PhotoDocs",
                table: "PhotoDocs",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PhotoDocs_PhotoDocumenations_PhotoDocumentationId",
                table: "PhotoDocs",
                column: "PhotoDocumentationId",
                principalTable: "PhotoDocumenations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PhotoDocs_PhotoDocumenations_PhotoDocumentationId",
                table: "PhotoDocs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PhotoDocs",
                table: "PhotoDocs");

            migrationBuilder.RenameTable(
                name: "PhotoDocs",
                newName: "PhotoDoc");

            migrationBuilder.RenameColumn(
                name: "PhotoDocumentationId",
                table: "PhotoDoc",
                newName: "PhotoDocumenationId");

            migrationBuilder.RenameIndex(
                name: "IX_PhotoDocs_PhotoDocumentationId",
                table: "PhotoDoc",
                newName: "IX_PhotoDoc_PhotoDocumenationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PhotoDoc",
                table: "PhotoDoc",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PhotoDoc_PhotoDocumenations_PhotoDocumenationId",
                table: "PhotoDoc",
                column: "PhotoDocumenationId",
                principalTable: "PhotoDocumenations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
