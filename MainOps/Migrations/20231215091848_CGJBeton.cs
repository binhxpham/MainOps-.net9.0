using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MainOps.Migrations
{
    public partial class CGJBeton : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DagsRapporterBeton",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SubProjectId = table.Column<int>(nullable: true),
                    ProjectId = table.Column<int>(nullable: true),
                    Dato = table.Column<DateTime>(nullable: false),
                    UdarbjedetAf = table.Column<string>(nullable: true),
                    ForSaetter = table.Column<bool>(nullable: false),
                    Vejr = table.Column<bool>(nullable: false),
                    Sol = table.Column<bool>(nullable: false),
                    Skyet = table.Column<bool>(nullable: false),
                    OverSkyet = table.Column<bool>(nullable: false),
                    Regn_sne = table.Column<bool>(nullable: false),
                    Vindstille = table.Column<bool>(nullable: false),
                    Svag_Vind = table.Column<bool>(nullable: false),
                    Jaevn_Vind = table.Column<bool>(nullable: false),
                    Haard_Vind = table.Column<bool>(nullable: false),
                    Temperatur_kl_otte = table.Column<double>(nullable: true),
                    Temperatur_kl_tolv = table.Column<double>(nullable: true),
                    UddybendeNoter = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DagsRapporterBeton", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DagsRapporterBeton_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DagsRapporterBeton_SubProjects_SubProjectId",
                        column: x => x.SubProjectId,
                        principalTable: "SubProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Materieller",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Materiellet = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materieller", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "imeRegistreringerEkstraBeton",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DagsRapportBetonId = table.Column<int>(nullable: true),
                    Navn = table.Column<string>(nullable: true),
                    AntalPersoner = table.Column<int>(nullable: false),
                    Timer = table.Column<double>(nullable: false),
                    Overtid_50100 = table.Column<double>(nullable: false),
                    Timer_EA = table.Column<double>(nullable: true),
                    Note = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_imeRegistreringerEkstraBeton", x => x.Id);
                    table.ForeignKey(
                        name: "FK_imeRegistreringerEkstraBeton_DagsRapporterBeton_DagsRapportB~",
                        column: x => x.DagsRapportBetonId,
                        principalTable: "DagsRapporterBeton",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KontraktarbejderBeton",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DagsRapportBetonId = table.Column<int>(nullable: true),
                    EgetMateriel = table.Column<string>(nullable: true),
                    UL_Firma = table.Column<string>(nullable: true),
                    UL_Materiel = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KontraktarbejderBeton", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KontraktarbejderBeton_DagsRapporterBeton_DagsRapportBetonId",
                        column: x => x.DagsRapportBetonId,
                        principalTable: "DagsRapporterBeton",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PhotoFilesBeton",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Path = table.Column<string>(nullable: true),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    DagsRapportBetonId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhotoFilesBeton", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhotoFilesBeton_DagsRapporterBeton_DagsRapportBetonId",
                        column: x => x.DagsRapportBetonId,
                        principalTable: "DagsRapporterBeton",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TimeRegistreringerBeton",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DagsRapportBetonId = table.Column<int>(nullable: true),
                    Navn = table.Column<string>(nullable: true),
                    AntalPersoner = table.Column<int>(nullable: false),
                    Timer = table.Column<double>(nullable: false),
                    Overtid_50100 = table.Column<double>(nullable: false),
                    Timer_EA = table.Column<double>(nullable: true),
                    Note = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeRegistreringerBeton", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimeRegistreringerBeton_DagsRapporterBeton_DagsRapportBetonId",
                        column: x => x.DagsRapportBetonId,
                        principalTable: "DagsRapporterBeton",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MaterielAntaller",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MaterielId = table.Column<int>(nullable: true),
                    Antal = table.Column<int>(nullable: false),
                    DagsRapportBetonId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterielAntaller", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaterielAntaller_DagsRapporterBeton_DagsRapportBetonId",
                        column: x => x.DagsRapportBetonId,
                        principalTable: "DagsRapporterBeton",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaterielAntaller_Materieller_MaterielId",
                        column: x => x.MaterielId,
                        principalTable: "Materieller",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DagsRapporterBeton_ProjectId",
                table: "DagsRapporterBeton",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_DagsRapporterBeton_SubProjectId",
                table: "DagsRapporterBeton",
                column: "SubProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_imeRegistreringerEkstraBeton_DagsRapportBetonId",
                table: "imeRegistreringerEkstraBeton",
                column: "DagsRapportBetonId");

            migrationBuilder.CreateIndex(
                name: "IX_KontraktarbejderBeton_DagsRapportBetonId",
                table: "KontraktarbejderBeton",
                column: "DagsRapportBetonId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterielAntaller_DagsRapportBetonId",
                table: "MaterielAntaller",
                column: "DagsRapportBetonId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterielAntaller_MaterielId",
                table: "MaterielAntaller",
                column: "MaterielId");

            migrationBuilder.CreateIndex(
                name: "IX_PhotoFilesBeton_DagsRapportBetonId",
                table: "PhotoFilesBeton",
                column: "DagsRapportBetonId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeRegistreringerBeton_DagsRapportBetonId",
                table: "TimeRegistreringerBeton",
                column: "DagsRapportBetonId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "imeRegistreringerEkstraBeton");

            migrationBuilder.DropTable(
                name: "KontraktarbejderBeton");

            migrationBuilder.DropTable(
                name: "MaterielAntaller");

            migrationBuilder.DropTable(
                name: "PhotoFilesBeton");

            migrationBuilder.DropTable(
                name: "TimeRegistreringerBeton");

            migrationBuilder.DropTable(
                name: "Materieller");

            migrationBuilder.DropTable(
                name: "DagsRapporterBeton");
        }
    }
}
