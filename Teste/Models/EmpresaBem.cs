namespace Teste.Models
{
    public class EmpresaBem
    {
        public int EmpresaId { get; set; }
        public Empresa Empresa { get; set; }
        public int BemId { get; set; }
        public Bem Bem { get; set; }
        public string Tipo { get; set; } = string.Empty;
    }
}
