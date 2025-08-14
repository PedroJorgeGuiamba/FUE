using System.ComponentModel.DataAnnotations;

namespace Teste.Models
{
    public class Localizacao
    {
        public int LocalizacaoId { get; set; }

        [Required(ErrorMessage = "Província é obrigatória.")]
        public string Provincia { get; set; } = string.Empty;

        [Required(ErrorMessage = "Distrito é obrigatório.")]
        public string Distrito { get; set; } = string.Empty;

        [Required(ErrorMessage = "Bairro é obrigatório.")]
        public string Bairro { get; set; } = string.Empty;

        [Required(ErrorMessage = "Avenida/Rua é obrigatória.")]
        public string AvenidaRua { get; set; } = string.Empty;

        [Required(ErrorMessage = "Número é obrigatório.")]
        public int Numero { get; set; }

        public float Latitude { get; set; }

        public float Longitude { get; set; }

        public string? Referencia { get; set; } // Optional
        //public int LocalizacaoId { get; set; }
        //public string Provincia { get; set; }
        //public string Distrito { get; set; }
        //public string Bairro { get; set; }
        //public string AvenidaRua { get; set; }
        //public int Numero {  get; set; }
        //public float Latitude { get; set; }
        //public float Longitude { get; set; }
        //public string Referencia { get; set; }
    }
}
