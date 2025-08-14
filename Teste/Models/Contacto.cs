using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Teste.Models
{
    public class Contacto
    {
        public int ContactoID { get; set; }

        public string? Fax1 { get; set; } // Optional
        public string? Fax2 { get; set; } // Optional
        public string? Telemovel1 { get; set; } // Optional
        public string? Telemovel2 { get; set; } // Optional
        public string? Telemovel3 { get; set; } // Optional

        [Required(ErrorMessage = "Email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Email inválido.")]
        public string Email { get; set; } = string.Empty;

        public string? Website { get; set; } // Optional

        [Required(ErrorMessage = "Sede é obrigatória.")]
        public int SedeId { get; set; }

        [ForeignKey("SedeId")]
        public Sede Sede { get; set; } = null!;
        //public string Fax1 { get; set; }    
        //public string Fax2 { get; set; }
        //public string Telemovel1 { get; set; }
        //public string Telemovel2 { get; set;}
        //public string Telemovel3 { get; set; }
        //public string Email { get; set; }
        //public string Website { get; set; }
        //public int SedeId { get; set; } // chave estrangeira
        //public Sede Sede { get; set; }

    }
}
