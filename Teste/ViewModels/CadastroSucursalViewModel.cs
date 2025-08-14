using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;
using System.ComponentModel.DataAnnotations;

namespace Teste.ViewModels
{
    public class CadastroSucursalViewModel
    {        // Bloco AA1 - Estabelecimento/Sucursal
        public int EmpresaId { get; set; }

        public string? NUIT_Sucursal { get; set; }
        public string? Nome_Sucursal { get; set; }
        public string? Sigla_Sucursal { get; set; }
        public string? NumeroAlvara_Sucursal { get; set; }
        public int? AnoConstituicao_Sucursal { get; set; }
        public int? DataInicioAno_Sucursal { get; set; }
        public int? DataInicioMes_Sucursal { get; set; }

        // Bloco AA2 - Localização do Estabelecimento/Sucursal
        public string? Provincia_Sucursal { get; set; }
        public string? Distrito_Sucursal { get; set; }
        public string? Bairro_Sucursal { get; set; }
        public string? AvenidaRua_Sucursal { get; set; }
        public int? Numero_Sucursal { get; set; }
        public float? Latitude_Sucursal { get; set; }
        public float? Longitude_Sucursal { get; set; }
        public string? Referencia_Sucursal { get; set; }

        // Bloco AA3 - Contactos do Estabelecimento/Sucursal
        public string? Fax1_Sucursal { get; set; }
        public string? Fax2_Sucursal { get; set; }
        public string? Telemovel1_Sucursal { get; set; }
        public string? Telemovel2_Sucursal { get; set; }
        public string? Telemovel3_Sucursal { get; set; }
        public string? Email_Sucursal { get; set; }
        public string? Website_Sucursal { get; set; }

        // Bloco AA4 - Responsável do Estabelecimento/Sucursal
        public string? NomeResponsavel_Sucursal { get; set; }
        public string? FuncaoResponsavel_Sucursal { get; set; }
        public string? TelemovelResponsavel_Sucursal { get; set; }
        public string? EmailResponsavel_Sucursal { get; set; }

        // Bloco BB1 - Caracterização do Estabelecimento/Sucursal
        public int? NumTrabalhadoresHomens_Sucursal { get; set; }
        public int? NumTrabalhadoresMulheres_Sucursal { get; set; }
        public string? TipoEntidade_Sucursal { get; set; }
        public string? SituacaoActividade_Sucursal { get; set; }
        public string? GrupoEmpresarial_Sucursal { get; set; }
        public string? NomeGrupoEmpresarial_Sucursal { get; set; }
        public string? PaisGrupoEmpresarial_Sucursal { get; set; }
        public IEnumerable<SelectListItem> TipoEntidades_Sucursal { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> GrupoEmpresarials_Sucursal { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> SituacaoActividades_Sucursal { get; set; } = new List<SelectListItem>();

       

    }
}
