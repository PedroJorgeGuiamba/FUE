using System.ComponentModel.DataAnnotations;

namespace Teste.Models
{
    public class Empresa : Sede
    {

        [Required(ErrorMessage = "Sucursal no País é obrigatória.")]
        public string SucursalNoPais { get; set; } = string.Empty;

        public int QuantidadeSucursalNoPais { get; set; }

        [Required(ErrorMessage = "Tipo de Contabilidade é obrigatório.")]
        public string TipoContabilidade { get; set; } = string.Empty;

        [Required(ErrorMessage = "Forma Jurídica é obrigatória.")]
        public string FormaJuridica { get; set; } = string.Empty;

        public double VolumeNegocios { get; set; }

        public double Despesas { get; set; }

        public double CapitalSocial { get; set; }

        [Range(0, 100, ErrorMessage = "Capital Social Público deve estar entre 0 e 100.")]
        public double CapitalSocialPublico { get; set; }

        [Range(0, 100, ErrorMessage = "Capital Privado Nacional deve estar entre 0 e 100.")]
        public double CapitalPrivadoNacional { get; set; }

        [Range(0, 100, ErrorMessage = "Capital Privado Estrangeiro deve estar entre 0 e 100.")]
        public double CapitalPrivadoEstrangeiro { get; set; }
        //public string SucursalNoPais { get; set; } = string.Empty;
        //public int QuantidadeSucursalNoPais { get; set; }
        //public string TipoContabilidade { get; set; } = string.Empty;
        //public string FormaJuridica { get; set; } = string.Empty;
        //public double VolumeNegocios { get; set; } 
        //public double Despesas { get; set; }
        //public double CapitalSocial { get; set; }
        //public double CapitalSocialPublico { get; set; }
        //public double CapitalPrivadoNacional { get; set; }
        //public double CapitalPrivadoEstrangeiro { get; set; }
        public ICollection<Sucursal> Sucursais { get; set; } = new List<Sucursal>();
        public ICollection<ActividadeEmpresa> Actividades { get; set; } = new List<ActividadeEmpresa>();
        public ICollection<EmpresaBem> Bens { get; set; } = new List<EmpresaBem>();

    }
}
