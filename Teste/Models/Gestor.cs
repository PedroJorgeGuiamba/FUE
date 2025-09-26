using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Teste.Models
{
    public class Gestor
    {
        [Key]
        public int GestorId { get; set; }
        [Required(ErrorMessage = "Nacionalidade é obrigatória.")]
        public string Nacionalidade { get; set; } = string.Empty;

        [Required(ErrorMessage = "Gênero é obrigatório.")]
        public string Genero { get; set; } = string.Empty;

        [Required(ErrorMessage = "Idade é obrigatória.")]
        public int Idade { get; set; }

        [Required(ErrorMessage = "Sede é obrigatória.")]
        public int SedeId { get; set; }

        [ForeignKey("SedeId")]
        [Required]
        public Sede Sede { get; set; } = null!;
    }
}