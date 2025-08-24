using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Teste.Models
{
    public class Sede
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "NUIT é obrigatório.")]
        public string NUIT { get; set; } // Non-nullable to align with ViewModel

        [Required(ErrorMessage = "Nome é obrigatório.")]
        public string Nome { get; set; } // Non-nullable

        [Required(ErrorMessage = "Sigla é obrigatória.")]
        public string Sigla { get; set; } // Non-nullable

        [Required(ErrorMessage = "Número do Alvará é obrigatório.")]
        public string NumeroAlvara { get; set; } // Non-nullable

        [Required(ErrorMessage = "Ano de Constituição é obrigatório.")]
        public int AnoConstituicao { get; set; } // Non-nullable

        [Required(ErrorMessage = "Data de Início (Ano) é obrigatória.")]
        public int DataInicioAno { get; set; } // Non-nullable

        [Required(ErrorMessage = "Data de Início (Mês) é obrigatória.")]
        public int DataInicioMes { get; set; } // Non-nullable

        [Required(ErrorMessage = "Localização é obrigatória.")]
        public int LocalizacaoId { get; set; }

        [ForeignKey("LocalizacaoId")]
        public Localizacao Localizacao { get; set; } = null!; // Non-null navigation property

        [Required(ErrorMessage = "Número de Trabalhadores Homens é obrigatório.")]
        public int NumTrabalhadoresHomens { get; set; } // Non-nullable

        [Required(ErrorMessage = "Número de Trabalhadores Mulheres é obrigatório.")]
        public int NumTrabalhadoresMulheres { get; set; } // Non-nullable

        [Required(ErrorMessage = "Tipo de Entidade é obrigatório.")]
        public string TipoEntidade { get; set; } = string.Empty;

        [Required(ErrorMessage = "Grupo Empresarial é obrigatório.")]
        public string GrupoEmpresarial { get; set; } = string.Empty;

        public string? NomeGrupoEmpresarial { get; set; } // Nullable, as it's optional

        public string? PaisGrupoEmpresarial { get; set; } // Nullable, as it's optional

        [Required(ErrorMessage = "Situação da Atividade é obrigatória.")]
        public string SituacaoActividade { get; set; } = string.Empty;

        public ICollection<Contacto> Contactos { get; set; }
        public ICollection<Responsavel> Responsaveis { get; set; } = new List<Responsavel>();

    }
}

//public int Id { get; set; }
//public string? NUIT { get; set; }
//public string? Nome { get; set; }
//public string? Sigla { get; set; }
//public string? NumeroAlvara { get; set; }
//public int? AnoConstituicao { get; set; }
//public int? DataInicioAno { get; set; }
//public int? DataInicioMes { get; set; }
//public int LocalizacaoId { get; set; }
//[ForeignKey("LocalizacaoId")]
//public Localizacao Localizacao { get; set; }
//public int? NumTrabalhadoresHomens { get; set; }
//public int? NumTrabalhadoresMulheres { get; set; }
//public string TipoEntidade { get; set; } = string.Empty;
//public string GrupoEmpresarial { get; set; } = string.Empty;
//public string? NomeGrupoEmpresarial { get; set; }
//public string? PaisGrupoEmpresarial { get; set; }
//public string SituacaoActividade { get; set; } = string.Empty;