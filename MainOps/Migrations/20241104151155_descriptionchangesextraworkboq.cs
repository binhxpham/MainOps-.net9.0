using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class descriptionchangesextraworkboq : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRelatedToBoQ",
                table: "ExtraWorkBoQDescriptions",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Topic",
                table: "ExtraWorkBoQDescriptions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRelatedToBoQ",
                table: "ExtraWorkBoQDescriptions");

            migrationBuilder.DropColumn(
                name: "Topic",
                table: "ExtraWorkBoQDescriptions");
        }
    }
}
