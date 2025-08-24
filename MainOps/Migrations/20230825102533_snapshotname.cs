using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class snapshotname : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SnapShotName",
                table: "SnapShots",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChangeLog",
                table: "SnapShotItems",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAltered",
                table: "SnapShotItems",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsNew",
                table: "SnapShotItems",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SnapShotName",
                table: "SnapShots");

            migrationBuilder.DropColumn(
                name: "ChangeLog",
                table: "SnapShotItems");

            migrationBuilder.DropColumn(
                name: "IsAltered",
                table: "SnapShotItems");

            migrationBuilder.DropColumn(
                name: "IsNew",
                table: "SnapShotItems");
        }
    }
}
