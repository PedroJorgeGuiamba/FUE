using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Teste.Migrations
{
    /// <inheritdoc />
    public partial class decimaTerceiraMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Primeiro, migrar dados existentes se necessário
            // Verificar se há registros que precisam ser migrados
            migrationBuilder.Sql(@"
                -- Verificar se existem responsáveis sem referência válida
                DELETE FROM Responsaveis 
                WHERE EmpresaId NOT IN (SELECT Id FROM Empresas)
                   OR (SucursalId IS NOT NULL AND SucursalId NOT IN (SELECT Id FROM Sucursais))
            ");

            // 2. Remover constraints antigas
            migrationBuilder.DropForeignKey(
                name: "FK_Responsaveis_Empresas_EmpresaId",
                table: "Responsaveis");

            migrationBuilder.DropForeignKey(
                name: "FK_Responsaveis_Sucursais_SucursalId",
                table: "Responsaveis");

            migrationBuilder.DropIndex(
                name: "IX_Responsaveis_SucursalId",
                table: "Responsaveis");

            migrationBuilder.DropColumn(
                name: "SucursalId",
                table: "Responsaveis");

            // 3. Adicionar nova coluna SedeId como nullable primeiro
            migrationBuilder.AddColumn<int>(
                name: "SedeId",
                table: "Responsaveis",
                type: "int",
                nullable: true);

            // 4. Migrar dados existentes para usar SedeId baseado em EmpresaId
            migrationBuilder.Sql(@"
                -- Assumindo que Empresa é uma Sede (precisa verificar seu modelo)
                -- Atualizar SedeId para referenciar a empresa
                UPDATE R SET SedeId = R.EmpresaId
                FROM Responsaveis R
                WHERE R.EmpresaId IN (SELECT Id FROM Empresas)
            ");

            // 5. Agora tornar SedeId obrigatório
            migrationBuilder.AlterColumn<int>(
                name: "SedeId",
                table: "Responsaveis",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            // 6. Criar índice
            migrationBuilder.CreateIndex(
                name: "IX_Responsaveis_SedeId",
                table: "Responsaveis",
                column: "SedeId");

            // 7. Adicionar foreign key
            migrationBuilder.AddForeignKey(
                name: "FK_Responsaveis_Sedes_SedeId",
                table: "Responsaveis",
                column: "SedeId",
                principalTable: "Sedes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            // 8. Verificar se o índice já existe antes de criar
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Responsaveis_EmpresaId' AND object_id = OBJECT_ID('Responsaveis'))
                BEGIN
                    CREATE INDEX [IX_Responsaveis_EmpresaId] ON [Responsaveis] ([EmpresaId]);
                END
            ");

            // 9. Adicionar foreign key para EmpresaId
            migrationBuilder.AddForeignKey(
                name: "FK_Responsaveis_Empresas_EmpresaId",
                table: "Responsaveis",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Responsaveis_Sedes_SedeId",
                table: "Responsaveis");

            migrationBuilder.DropForeignKey(
                name: "FK_Responsaveis_Empresas_EmpresaId",
                table: "Responsaveis");

            migrationBuilder.DropIndex(
                name: "IX_Responsaveis_SedeId",
                table: "Responsaveis");

            // Verificar se o índice existe antes de tentar removê-lo
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Responsaveis_EmpresaId' AND object_id = OBJECT_ID('Responsaveis'))
                BEGIN
                    DROP INDEX [IX_Responsaveis_EmpresaId] ON [Responsaveis];
                END
            ");

            migrationBuilder.DropColumn(
                name: "SedeId",
                table: "Responsaveis");

            migrationBuilder.AddColumn<int>(
                name: "SucursalId",
                table: "Responsaveis",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Responsaveis_EmpresaId",
                table: "Responsaveis",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_Responsaveis_SucursalId",
                table: "Responsaveis",
                column: "SucursalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Responsaveis_Empresas_EmpresaId",
                table: "Responsaveis",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Responsaveis_Sucursais_SucursalId",
                table: "Responsaveis",
                column: "SucursalId",
                principalTable: "Sucursais",
                principalColumn: "Id");
        }
    }
}
