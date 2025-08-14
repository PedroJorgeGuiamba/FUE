using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Teste.Migrations
{
    /// <inheritdoc />
    public partial class QuintaMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GrupoEmpresarial",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "NomeGrupoEmpresarial",
                table: "Empresas");

            migrationBuilder.AddColumn<string>(
                name: "GrupoEmpresarial",
                table: "Sedes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NomeGrupoEmpresarial",
                table: "Sedes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GrupoEmpresarial",
                table: "Sedes");

            migrationBuilder.DropColumn(
                name: "NomeGrupoEmpresarial",
                table: "Sedes");

            migrationBuilder.AddColumn<string>(
                name: "GrupoEmpresarial",
                table: "Empresas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NomeGrupoEmpresarial",
                table: "Empresas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
