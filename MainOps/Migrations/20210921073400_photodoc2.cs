using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class photodoc2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoPath",
                table: "PhotoDocumenations");

            migrationBuilder.CreateTable(
                name: "PhotoDoc",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Path = table.Column<string>(nullable: true),
                    PhotoDocumenationId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhotoDoc", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhotoDoc_PhotoDocumenations_PhotoDocumenationId",
                        column: x => x.PhotoDocumenationId,
                        principalTable: "PhotoDocumenations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PhotoDoc_PhotoDocumenationId",
                table: "PhotoDoc",
                column: "PhotoDocumenationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PhotoDoc");

            migrationBuilder.AddColumn<string>(
                name: "PhotoPath",
                table: "PhotoDocumenations",
                nullable: true);
        }
    }
}
