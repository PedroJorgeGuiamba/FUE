using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Teste.Models;

public class Sucursal : Sede
{
    public int SucursalId { get; set; }

    [Required(ErrorMessage = "Empresa é obrigatória.")]
    public int EmpresaId { get; set; }

    [ForeignKey("EmpresaId")]
    public Empresa Empresa { get; set; } = null!; 
}

// Non-null navigation property
//public int EmpresaId { get; set; } // chave estrangeira
//public Empresa Empresa { get; set; }