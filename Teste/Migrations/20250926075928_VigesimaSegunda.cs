using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Teste.Migrations
{
    /// <inheritdoc />
    public partial class VigesimaSegunda : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AnoEncerramento",
                table: "Sedes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ActividadeSucursais",
                columns: table => new
                {
                    SucursalId = table.Column<int>(type: "int", nullable: false),
                    ActividadeId = table.Column<int>(type: "int", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActividadeSucursais", x => new { x.SucursalId, x.ActividadeId });
                    table.ForeignKey(
                        name: "FK_ActividadeSucursais_Actividades_ActividadeId",
                        column: x => x.ActividadeId,
                        principalTable: "Actividades",
                        principalColumn: "ActividadeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActividadeSucursais_Sucursais_SucursalId",
                        column: x => x.SucursalId,
                        principalTable: "Sucursais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActividadeSucursais_ActividadeId",
                table: "ActividadeSucursais",
                column: "ActividadeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActividadeSucursais");

            migrationBuilder.DropColumn(
                name: "AnoEncerramento",
                table: "Sedes");
        }
    }
}
