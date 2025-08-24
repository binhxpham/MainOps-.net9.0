using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class mobilisationupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<double>(
            //    name: "PaidAmount",
            //    table: "Mobilisations",
            //    nullable: false,
            //    defaultValue: 0.0);

            //migrationBuilder.AddColumn<bool>(
            //    name: "ToBePaid",
            //    table: "Mobilisations",
            //    nullable: false,
            //    defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "UniqueID",
                table: "Mobilisations",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaidAmount",
                table: "Mobilisations");

            migrationBuilder.DropColumn(
                name: "ToBePaid",
                table: "Mobilisations");

            migrationBuilder.DropColumn(
                name: "UniqueID",
                table: "Mobilisations");
        }
    }
}
