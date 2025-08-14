using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Teste.Migrations
{
    /// <inheritdoc />
    public partial class decimaquinta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Remover foreign key
            migrationBuilder.DropForeignKey(
                name: "FK_Responsaveis_Empresas_EmpresaId",
                table: "Responsaveis");

            // Remover índice
            migrationBuilder.DropIndex(
                name: "IX_Responsaveis_EmpresaId",
                table: "Responsaveis");

            // Agora sim: remover a coluna
            migrationBuilder.DropColumn(
                name: "EmpresaId",
                table: "Responsaveis");
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EmpresaId",
                table: "Responsaveis",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
