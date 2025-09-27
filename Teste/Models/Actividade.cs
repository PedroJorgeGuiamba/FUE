namespace Teste.Models
{
    public class Actividade
    {
        public int ActividadeId { get; set; }
        public string Descricao { get; set; }
        public string CodigoCAE { get; set; }
        public ICollection<ActividadeEmpresa> ActividadeEmpresas { get; set; } = new List<ActividadeEmpresa>();
        public ICollection<ActividadeSucursal> ActividadeSucursais { get; set; } = new List<ActividadeSucursal>();
    }
}
