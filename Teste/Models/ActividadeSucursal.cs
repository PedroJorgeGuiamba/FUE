using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Teste.Models
{
    public class ActividadeSucursal
    {
        [Required]
        public int SucursalId { get; set; }

        [ForeignKey("SucursalId")]
        public Sucursal Sucursal { get; set; } = null!;

        [Required]
        public int ActividadeId { get; set; }

        [ForeignKey("ActividadeId")]
        public Actividade Actividade { get; set; } = null!;

        [Required]
        public string Tipo { get; set; } = string.Empty;
    }
}
