using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Teste.Models
{
    public class ActividadeEmpresa
    {
        [Required]
        public int EmpresaId { get; set; }

        [ForeignKey("EmpresaId")]
        public Empresa Empresa { get; set; } = null!;

        [Required]
        public int ActividadeId { get; set; }

        [ForeignKey("ActividadeId")]
        public Actividade Actividade { get; set; } = null!;

        [Required]
        public string Tipo { get; set; } = string.Empty;
    }
}