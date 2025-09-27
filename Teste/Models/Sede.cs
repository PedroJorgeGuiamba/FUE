using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Teste.Models
{
    public class Sede
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "NUIT é obrigatório.")]
        public string NUIT { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Sigla é obrigatória.")]
        public string Sigla { get; set; }

        [Required(ErrorMessage = "Número do Alvará é obrigatório.")]
        public string NumeroAlvara { get; set; }

        [Required(ErrorMessage = "Ano de Constituição é obrigatório.")]
        public int AnoConstituicao { get; set; }

        [Required(ErrorMessage = "Data de Início (Ano) é obrigatória.")]
        public int DataInicioAno { get; set; }

        [Required(ErrorMessage = "Data de Início (Mês) é obrigatória.")]
        public int DataInicioMes { get; set; }

        [Required(ErrorMessage = "Localização é obrigatória.")]
        public int LocalizacaoId { get; set; }

        [ForeignKey("LocalizacaoId")]
        public Localizacao Localizacao { get; set; } = null!;

        [Required(ErrorMessage = "Número de Trabalhadores Homens é obrigatório.")]
        public int NumTrabalhadoresHomens { get; set; }

        [Required(ErrorMessage = "Número de Trabalhadores Mulheres é obrigatório.")]
        public int NumTrabalhadoresMulheres { get; set; }

        [Required(ErrorMessage = "Tipo de Entidade é obrigatório.")]
        public string TipoEntidade { get; set; } = string.Empty;

        [Required(ErrorMessage = "Grupo Empresarial é obrigatório.")]
        public string GrupoEmpresarial { get; set; } = string.Empty;
        public string? NomeGrupoEmpresarial { get; set; }
        public string? PaisGrupoEmpresarial { get; set; }
        [Required(ErrorMessage = "Situação da Atividade é obrigatória.")]
        public string SituacaoActividade { get; set; } = string.Empty;
        public int? AnoEncerramento { get; set; } //Vai servir para armazzenar os anos dos que interromperram actividades ou encerraram
        public ICollection<Contacto> Contactos { get; set; }
        public ICollection<Responsavel> Responsaveis { get; set; } = new List<Responsavel>();
        public ICollection<Gestor> Gestores { get; set; } = new List<Gestor>();
    }
}