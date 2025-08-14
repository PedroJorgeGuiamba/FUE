using Microsoft.EntityFrameworkCore;
using Teste.Models;

namespace Teste.Data
{
    public class FueDbContext : DbContext
    {
        public FueDbContext(DbContextOptions<FueDbContext> options) : base(options) { }

        public DbSet<Sede> Sedes { get; set; }
        public DbSet<Empresa> Empresas { get; set; }
        public DbSet<Sucursal> Sucursais { get; set; }
        public DbSet<Contacto> Contactos { get; set; }
        public DbSet<Actividade> Actividades { get; set; }
        public DbSet<ActividadeEmpresa> ActividadeEmpresas { get; set; } // ADICIONE
        public DbSet<Localizacao> Localizacoes { get; set; }
        public DbSet<Responsavel> Responsaveis { get; set; }
        public DbSet<Bem> Bens {  get; set; }
        public DbSet<EmpresaBem> EmpresaBens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração para evitar erro de herança ambígua
            modelBuilder.Entity<Sede>().UseTptMappingStrategy(); // Table-per-type (opcional)

            // Configuração para ActividadeEmpresa como many-to-many (se necessário)
            modelBuilder.Entity<ActividadeEmpresa>()
                .HasKey(ae => new { ae.EmpresaId, ae.ActividadeId });

            modelBuilder.Entity<ActividadeEmpresa>()
                .HasOne(ae => ae.Empresa)
                .WithMany(e => e.Actividades)
                .HasForeignKey(ae => ae.EmpresaId);


            modelBuilder.Entity<ActividadeEmpresa>()
                .HasOne(ae => ae.Actividade)
                .WithMany()
                .HasForeignKey(ae => ae.ActividadeId);


            modelBuilder.Entity<EmpresaBem>()
                .HasKey(ae => new { ae.EmpresaId, ae.BemId });

            modelBuilder.Entity<EmpresaBem>()
                .HasOne(ae => ae.Empresa)
                .WithMany(e => e.Bens)
                .HasForeignKey(ae => ae.EmpresaId);


            modelBuilder.Entity<EmpresaBem>()
                .HasOne(ae => ae.Bem)
                .WithMany()
                .HasForeignKey(ae => ae.BemId);

            modelBuilder.Entity<Sucursal>()
                .HasOne(s => s.Empresa)
                .WithMany(e => e.Sucursais)
                .HasForeignKey(s => s.EmpresaId)
                .OnDelete(DeleteBehavior.Restrict);  // ou ClientSetNull


            modelBuilder.Entity<Sede>()
                .HasOne(s => s.Localizacao)
                .WithOne()
                .HasForeignKey<Sede>(s => s.LocalizacaoId);

            modelBuilder.Entity<Contacto>()
                .HasOne(c => c.Sede)
                .WithMany(s => s.Contactos)
                .HasForeignKey(c => c.SedeId);
            
            modelBuilder.Entity<Responsavel>()
                .HasOne(r => r.Sede)
                .WithMany(s => s.Responsaveis)
                .HasForeignKey(r => r.SedeId)
                .OnDelete(DeleteBehavior.Cascade); // ou ClientSetNull se preferir
        }
    }
}
