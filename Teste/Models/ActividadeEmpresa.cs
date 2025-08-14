using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Teste.Models
{
    public class ActividadeEmpresa
    {
        public int EmpresaId { get; set; }
        public Empresa Empresa { get; set; }
        public int ActividadeId { get; set; }
        public Actividade Actividade { get; set; }
        public string Tipo { get; set; } = string.Empty;

    }
}
