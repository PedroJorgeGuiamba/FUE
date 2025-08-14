using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Teste.ViewModels
{
    public class CadastroEmpresaViewModel
    {
        // Bloco A1 - Identificação da Entidade
        [Required(ErrorMessage = "NUIT é obrigatório.")]
        public string NUIT { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nome é obrigatório.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Sigla é obrigatória.")]
        public string Sigla { get; set; } = string.Empty;

        [Required(ErrorMessage = "Número do Alvará é obrigatório.")]
        public string NumeroAlvara { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ano de Constituição é obrigatório.")]
        public int AnoConstituicao { get; set; }

        [Required(ErrorMessage = "Data de Início (Ano) é obrigatória.")]
        public int DataInicioAno { get; set; }

        [Required(ErrorMessage = "Data de Início (Mês) é obrigatória.")]
        public int DataInicioMes { get; set; }

        [Required(ErrorMessage = "Número de Trabalhadores Homens é obrigatório.")]
        public int NumTrabalhadoresHomens { get; set; }

        [Required(ErrorMessage = "Número de Trabalhadores Mulheres é obrigatório.")]
        public int NumTrabalhadoresMulheres { get; set; }

        // Bloco A2 - Localização da Entidade
        [Required(ErrorMessage = "Província é obrigatória.")]
        public string Provincia { get; set; } = string.Empty;

        [Required(ErrorMessage = "Distrito é obrigatório.")]
        public string Distrito { get; set; } = string.Empty;

        public string? PostoAdministrativo { get; set; }
        public string? Localidade { get; set; }
        public string? Povoado { get; set; }

        [Required(ErrorMessage = "Bairro é obrigatório.")]
        public string Bairro { get; set; } = string.Empty;

        [Required(ErrorMessage = "Avenida/Rua é obrigatória.")]
        public string AvenidaRua { get; set; } = string.Empty;

        [Required(ErrorMessage = "Número é obrigatório.")]
        public int Numero { get; set; }

        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public string? Referencia { get; set; }

        // Bloco A3 - Contactos da Entidade
        public string? Fax1 { get; set; }
        public string? Fax2 { get; set; }
        public string? Telemovel1 { get; set; }
        public string? Telemovel2 { get; set; }
        public string? Telemovel3 { get; set; }

        [Required(ErrorMessage = "Email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Email inválido.")]
        public string Email { get; set; } = string.Empty;

        public string? Website { get; set; }

        // Bloco A4 - Responsável pelo Preenchimento
        [Required(ErrorMessage = "Nome do Responsável é obrigatório.")]
        public string NomeResponsavel { get; set; } = string.Empty;

        [Required(ErrorMessage = "Função do Responsável é obrigatória.")]
        public string FuncaoResponsavel { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telemóvel do Responsável é obrigatório.")]
        public string TelemovelResponsavel { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email do Responsável é obrigatório.")]
        [EmailAddress(ErrorMessage = "Email inválido.")]
        public string EmailResponsavel { get; set; } = string.Empty;

        // Bloco B1 - Caracterização da Entidade
        [Required(ErrorMessage = "Sucursal no País é obrigatória.")]
        public string SucursalNoPais { get; set; } = string.Empty;

        [Range(0, int.MaxValue, ErrorMessage = "Quantidade de Sucursais deve ser maior ou igual a 0.")]
        public int QuantidadeSucursalNoPais { get; set; }

        [Required(ErrorMessage = "Tipo de Entidade é obrigatório.")]
        public string TipoEntidade { get; set; } = string.Empty;

        [Required(ErrorMessage = "Forma Jurídica é obrigatória.")]
        public string FormaJuridica { get; set; } = string.Empty;

        [Required(ErrorMessage = "Situação da Atividade é obrigatória.")]
        public string SituacaoActividade { get; set; } = string.Empty;

        [Required(ErrorMessage = "Grupo Empresarial é obrigatório.")]
        public string GrupoEmpresarial { get; set; } = string.Empty;

        public string? NomeGrupoEmpresarial { get; set; }
        public string? PaisGrupoEmpresarial { get; set; }

        [Required(ErrorMessage = "Tipo de Contabilidade é obrigatório.")]
        public string TipoContabilidade { get; set; } = string.Empty;

        [Range(0, double.MaxValue, ErrorMessage = "Volume de Negócios deve ser maior ou igual a 0.")]
        public double VolumeNegocios { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Despesas devem ser maiores ou iguais a 0.")]
        public double Despesas { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Capital Social deve ser maior ou igual a 0.")]
        public double CapitalSocial { get; set; }

        [Range(0, 100, ErrorMessage = "Capital Social Público deve estar entre 0 e 100.")]
        public double CapitalSocialPublico { get; set; }

        [Range(0, 100, ErrorMessage = "Capital Privado Nacional deve estar entre 0 e 100.")]
        public double CapitalPrivadoNacional { get; set; }

        [Range(0, 100, ErrorMessage = "Capital Privado Estrangeiro deve estar entre 0 e 100.")]
        public double CapitalPrivadoEstrangeiro { get; set; }

        // Dropdown Lists
        public IEnumerable<SelectListItem> SucursalNosPaises { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> TipoEntidades { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> FormaJuridicas { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> SituacaoActividades { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> GrupoEmpresarials { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> TipoContabilidades { get; set; } = new List<SelectListItem>();



        // Custom Validation for Capital Percentages
        [CustomValidation(typeof(CadastroEmpresaViewModel), nameof(ValidateCapitalPercentages))]
        public double TotalCapitalPercentages => CapitalSocialPublico + CapitalPrivadoNacional + CapitalPrivadoEstrangeiro;

        public static ValidationResult ValidateCapitalPercentages(double total, ValidationContext context)
        {
            if (Math.Abs(total - 100.0) > 0.01)
            {
                return new ValidationResult("A soma dos percentuais de capital deve ser igual a 100%.");
            }
            return ValidationResult.Success!;
        }
    }
}