using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Teste.Migrations
{
    /// <inheritdoc />
    public partial class PrimeiraMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Actividades",
                columns: table => new
                {
                    ActividadeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CodigoCAE = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actividades", x => x.ActividadeId);
                });

            migrationBuilder.CreateTable(
                name: "Bens",
                columns: table => new
                {
                    BemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CodigoCNBS = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bens", x => x.BemId);
                });

            migrationBuilder.CreateTable(
                name: "Localizacoes",
                columns: table => new
                {
                    LocalizacaoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Provincia = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Distrito = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Bairro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AvenidaRua = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Numero = table.Column<int>(type: "int", nullable: false),
                    Latitude = table.Column<float>(type: "real", nullable: false),
                    Longitude = table.Column<float>(type: "real", nullable: false),
                    Referencia = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Localizacoes", x => x.LocalizacaoId);
                });

            migrationBuilder.CreateTable(
                name: "Sedes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NUIT = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sigla = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumeroAlvara = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AnoConstituicao = table.Column<int>(type: "int", nullable: true),
                    DataInicioAno = table.Column<int>(type: "int", nullable: true),
                    DataInicioMes = table.Column<int>(type: "int", nullable: true),
                    LocalizacaoId = table.Column<int>(type: "int", nullable: false),
                    NumTrabalhadoresHomens = table.Column<int>(type: "int", nullable: true),
                    NumTrabalhadoresMulheres = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sedes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sedes_Localizacoes_LocalizacaoId",
                        column: x => x.LocalizacaoId,
                        principalTable: "Localizacoes",
                        principalColumn: "LocalizacaoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Contactos",
                columns: table => new
                {
                    ContactoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fax1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fax2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telemovel1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telemovel2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telemovel3 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Website = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SedeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contactos", x => x.ContactoID);
                    table.ForeignKey(
                        name: "FK_Contactos_Sedes_SedeId",
                        column: x => x.SedeId,
                        principalTable: "Sedes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Empresas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    EmpresaId = table.Column<int>(type: "int", nullable: false),
                    SucursalNoPais = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuantidadeSucursalNoPais = table.Column<int>(type: "int", nullable: false),
                    TipoContabilidade = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FormaJuridica = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SituacaoActividade = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GrupoEmpresarial = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VolumeNegocios = table.Column<double>(type: "float", nullable: false),
                    Despesas = table.Column<double>(type: "float", nullable: false),
                    CapitalSocialPublico = table.Column<double>(type: "float", nullable: false),
                    CapitalPrivadoNacional = table.Column<double>(type: "float", nullable: false),
                    CapitalPrivadoEstrangeiro = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empresas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Empresas_Sedes_Id",
                        column: x => x.Id,
                        principalTable: "Sedes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActividadeEmpresas",
                columns: table => new
                {
                    EmpresaId = table.Column<int>(type: "int", nullable: false),
                    ActividadeId = table.Column<int>(type: "int", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActividadeEmpresas", x => new { x.EmpresaId, x.ActividadeId });
                    table.ForeignKey(
                        name: "FK_ActividadeEmpresas_Actividades_ActividadeId",
                        column: x => x.ActividadeId,
                        principalTable: "Actividades",
                        principalColumn: "ActividadeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActividadeEmpresas_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmpresaBens",
                columns: table => new
                {
                    EmpresaId = table.Column<int>(type: "int", nullable: false),
                    BemId = table.Column<int>(type: "int", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpresaBens", x => new { x.EmpresaId, x.BemId });
                    table.ForeignKey(
                        name: "FK_EmpresaBens_Bens_BemId",
                        column: x => x.BemId,
                        principalTable: "Bens",
                        principalColumn: "BemId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmpresaBens_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sucursais",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    SucursalId = table.Column<int>(type: "int", nullable: false),
                    EmpresaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sucursais", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sucursais_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sucursais_Sedes_Id",
                        column: x => x.Id,
                        principalTable: "Sedes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Responsaveis",
                columns: table => new
                {
                    ResponsavelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpresaId = table.Column<int>(type: "int", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Funcao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telemovel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SucursalId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Responsaveis", x => x.ResponsavelId);
                    table.ForeignKey(
                        name: "FK_Responsaveis_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Responsaveis_Sucursais_SucursalId",
                        column: x => x.SucursalId,
                        principalTable: "Sucursais",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActividadeEmpresas_ActividadeId",
                table: "ActividadeEmpresas",
                column: "ActividadeId");

            migrationBuilder.CreateIndex(
                name: "IX_Contactos_SedeId",
                table: "Contactos",
                column: "SedeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpresaBens_BemId",
                table: "EmpresaBens",
                column: "BemId");

            migrationBuilder.CreateIndex(
                name: "IX_Responsaveis_EmpresaId",
                table: "Responsaveis",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_Responsaveis_SucursalId",
                table: "Responsaveis",
                column: "SucursalId");

            migrationBuilder.CreateIndex(
                name: "IX_Sedes_LocalizacaoId",
                table: "Sedes",
                column: "LocalizacaoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sucursais_EmpresaId",
                table: "Sucursais",
                column: "EmpresaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActividadeEmpresas");

            migrationBuilder.DropTable(
                name: "Contactos");

            migrationBuilder.DropTable(
                name: "EmpresaBens");

            migrationBuilder.DropTable(
                name: "Responsaveis");

            migrationBuilder.DropTable(
                name: "Actividades");

            migrationBuilder.DropTable(
                name: "Bens");

            migrationBuilder.DropTable(
                name: "Sucursais");

            migrationBuilder.DropTable(
                name: "Empresas");

            migrationBuilder.DropTable(
                name: "Sedes");

            migrationBuilder.DropTable(
                name: "Localizacoes");
        }
    }
}
