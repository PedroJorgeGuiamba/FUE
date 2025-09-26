using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Teste.Models
{
    public class Responsavel
    {
        public int ResponsavelId { get; set; }
        [Required(ErrorMessage = "Nome é obrigatório.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Função é obrigatória.")]
        public string Funcao { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telemóvel é obrigatório.")]
        public string Telemovel { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Email inválido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Sede é obrigatória.")]
        public int SedeId { get; set; }

        [ForeignKey("SedeId")]
        public Sede Sede { get; set; } = null!;
    }
}
