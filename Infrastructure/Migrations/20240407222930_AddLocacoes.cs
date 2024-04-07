using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class AddLocacoes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Locacoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImovelId = table.Column<int>(type: "int", nullable: false),
                    LocadorId = table.Column<int>(type: "int", nullable: false),
                    LocatarioId = table.Column<int>(type: "int", nullable: false),
                    LocadorAssinou = table.Column<bool>(type: "bit", nullable: false),
                    LocatarioAssinou = table.Column<bool>(type: "bit", nullable: false),
                    DataFechamento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataVencimento = table.Column<DateTime>(type: "date", nullable: false),
                    ValorMensal = table.Column<decimal>(type: "decimal(12,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Locacoes_Imoveis_ImovelId",
                        column: x => x.ImovelId,
                        principalTable: "Imoveis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Locacoes_Usuarios_LocadorId",
                        column: x => x.LocadorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Locacoes_Usuarios_LocatarioId",
                        column: x => x.LocatarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Locacoes_ImovelId",
                table: "Locacoes",
                column: "ImovelId");

            migrationBuilder.CreateIndex(
                name: "IX_Locacoes_LocadorId",
                table: "Locacoes",
                column: "LocadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Locacoes_LocatarioId",
                table: "Locacoes",
                column: "LocatarioId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Locacoes");
        }
    }
}
