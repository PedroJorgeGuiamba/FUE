using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Teste.Migrations
{
    /// <inheritdoc />
    public partial class SextaMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaisGrupoEmpresarial",
                table: "Empresas");

            migrationBuilder.AddColumn<string>(
                name: "PaisGrupoEmpresarial",
                table: "Sedes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaisGrupoEmpresarial",
                table: "Sedes");

            migrationBuilder.AddColumn<string>(
                name: "PaisGrupoEmpresarial",
                table: "Empresas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
