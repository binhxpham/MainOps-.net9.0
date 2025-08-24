using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class projectstatusprojectcategories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectStatuses_ProjectCategories_ProjectCategoryId",
                table: "ProjectStatuses");

            migrationBuilder.DropIndex(
                name: "IX_ProjectStatuses_ProjectCategoryId",
                table: "ProjectStatuses");

            migrationBuilder.DropColumn(
                name: "ProjectCategoryId",
                table: "ProjectStatuses");

            migrationBuilder.CreateTable(
                name: "ProjectStatusProjectCategories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    ProjectStatusId = table.Column<int>(nullable: false),
                    ProjectCategoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectStatusProjectCategories", x => new { x.ProjectStatusId, x.ProjectCategoryId });
                    table.UniqueConstraint("AK_ProjectStatusProjectCategories_Id", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectStatusProjectCategories_ProjectCategories_ProjectCate~",
                        column: x => x.ProjectCategoryId,
                        principalTable: "ProjectCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectStatusProjectCategories_ProjectStatuses_ProjectStatus~",
                        column: x => x.ProjectStatusId,
                        principalTable: "ProjectStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectStatusProjectCategories_ProjectCategoryId",
                table: "ProjectStatusProjectCategories",
                column: "ProjectCategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectStatusProjectCategories");

            migrationBuilder.AddColumn<int>(
                name: "ProjectCategoryId",
                table: "ProjectStatuses",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectStatuses_ProjectCategoryId",
                table: "ProjectStatuses",
                column: "ProjectCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectStatuses_ProjectCategories_ProjectCategoryId",
                table: "ProjectStatuses",
                column: "ProjectCategoryId",
                principalTable: "ProjectCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
