using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class worktasksnotbound : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkItemPhotos");

            migrationBuilder.DropTable(
                name: "WorkItems");

            migrationBuilder.RenameColumn(
                name: "WorkerId",
                table: "WorkTasks",
                newName: "InCharge");

            migrationBuilder.RenameColumn(
                name: "InChargeId",
                table: "WorkTasks",
                newName: "ApplicationUserId");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "WorkTasks",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkTasks_ApplicationUserId",
                table: "WorkTasks",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkTasks_AspNetUsers_ApplicationUserId",
                table: "WorkTasks",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
