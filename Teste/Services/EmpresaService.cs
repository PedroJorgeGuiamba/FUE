using Teste.Data;
using Teste.Models;
using Microsoft.EntityFrameworkCore;

namespace Teste.Services
{
    public class EmpresaService
    {
        private readonly FueDbContext _context;

        public EmpresaService(FueDbContext context)
        {
            _context = context;
        }

        public async Task<List<Empresa>> GetAllAsync()
        {
            return await _context.Empresas
                .Include(e => e.Sucursais)
                .Include(e => e.Actividades)
                .Include(e => e.Bens)
                .ToListAsync();
        }

        public async Task<Empresa> GetByIdAsync(int id)
        {
            return await _context.Empresas.FindAsync(id);
        }

        public async Task AddAsync(Empresa empresa)
        {
            _context.Empresas.Add(empresa);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Empresa empresa)
        {
            _context.Entry(empresa).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<Sede> GetSedeByIdAsync(int id)
        {
            return await _context.Sedes
                .Include(s => s.Contactos)
                .Include(s => s.Responsaveis)
                .Include(s => s.Gestores)
                .Include(s => s.Localizacao)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Primeiro remove os registros das tabelas de relacionamento many-to-many
                await RemoveActividadeEmpresasAsync(id);
                await RemoveEmpresaBensAsync(id);

                // Verifica se é uma Sede para remover relacionamentos específicos
                var isSede = await _context.Sedes.AnyAsync(s => s.Id == id);

                if (isSede)
                {
                    return await DeleteSedeAsync(id);
                }
                else
                {
                    return await DeleteEmpresaAsync(id);
                }
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Erro de banco de dados ao excluir empresa: {ex.InnerException?.Message ?? ex.Message}", ex);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Erro inesperado ao excluir empresa: {ex.Message}", ex);
            }
        }

        private async Task RemoveActividadeEmpresasAsync(int empresaId)
        {
            var actividadesEmpresa = await _context.ActividadeEmpresas
                .Where(ae => ae.EmpresaId == empresaId)
                .ToListAsync();

            if (actividadesEmpresa.Any())
            {
                _context.ActividadeEmpresas.RemoveRange(actividadesEmpresa);
                await _context.SaveChangesAsync();
            }
        }

        private async Task RemoveEmpresaBensAsync(int empresaId)
        {
            var empresaBens = await _context.EmpresaBens
                .Where(eb => eb.EmpresaId == empresaId)
                .ToListAsync();

            if (empresaBens.Any())
            {
                _context.EmpresaBens.RemoveRange(empresaBens);
                await _context.SaveChangesAsync();
            }
        }

        private async Task<bool> DeleteSedeAsync(int sedeId)
        {
            var sede = await _context.Sedes
                .Include(s => s.Contactos)
                .Include(s => s.Responsaveis)
                .Include(s => s.Gestores)
                .Include(s => s.Localizacao)
                .FirstOrDefaultAsync(s => s.Id == sedeId);

            if (sede == null) return false;

            // Remove Contactos
            if (sede.Contactos?.Any() == true)
            {
                _context.Contactos.RemoveRange(sede.Contactos);
            }

            // Remove Responsaveis
            if (sede.Responsaveis?.Any() == true)
            {
                _context.Responsaveis.RemoveRange(sede.Responsaveis);
            }

            // Remove Gestores
            if (sede.Gestores?.Any() == true)
            {
                _context.Gestores.RemoveRange(sede.Gestores);
            }

            // Remove Sucursais relacionadas
            var sucursais = await _context.Sucursais
                .Where(s => s.EmpresaId == sedeId)
                .ToListAsync();

            if (sucursais.Any())
            {
                // Remove ActividadeEmpresas e EmpresaBens das sucursais primeiro
                foreach (var sucursal in sucursais)
                {
                    await RemoveActividadeEmpresasAsync(sucursal.Id);
                    await RemoveEmpresaBensAsync(sucursal.Id);
                }

                _context.Sucursais.RemoveRange(sucursais);
            }

            // Remove a Sede
            _context.Sedes.Remove(sede);
            await _context.SaveChangesAsync();

            return true;
        }

        private async Task<bool> DeleteEmpresaAsync(int empresaId)
        {
            var empresa = await _context.Empresas
                .Include(e => e.Sucursais)
                .Include(e => e.Localizacao)
                .FirstOrDefaultAsync(e => e.Id == empresaId);

            if (empresa == null) return false;

            // Remove Sucursais
            if (empresa.Sucursais?.Any() == true)
            {
                // Remove ActividadeEmpresas e EmpresaBens das sucursais primeiro
                foreach (var sucursal in empresa.Sucursais)
                {
                    await RemoveActividadeEmpresasAsync(sucursal.Id);
                    await RemoveEmpresaBensAsync(sucursal.Id);
                }

                _context.Sucursais.RemoveRange(empresa.Sucursais);
            }
            // Remove a Empresa
            _context.Empresas.Remove(empresa);
            await _context.SaveChangesAsync();

            return true;
        }

        // Métodos auxiliares para contar dependências
        public async Task<int> GetSucursaisCountAsync(int empresaId)
        {
            return await _context.Sucursais.CountAsync(s => s.EmpresaId == empresaId);
        }

        public async Task<int> GetActividadesCountAsync(int empresaId)
        {
            return await _context.ActividadeEmpresas
                .Where(ae => ae.EmpresaId == empresaId)
                .CountAsync();
        }

        public async Task<int> GetBensCountAsync(int empresaId)
        {
            return await _context.EmpresaBens
                .Where(eb => eb.EmpresaId == empresaId)
                .CountAsync();
        }

        public async Task<int> GetContactosCountAsync(int sedeId)
        {
            return await _context.Contactos.CountAsync(c => c.SedeId == sedeId);
        }

        public async Task<int> GetResponsaveisCountAsync(int sedeId)
        {
            return await _context.Responsaveis.CountAsync(r => r.SedeId == sedeId);
        }

        public async Task<int> GetGestoresCountAsync(int sedeId)
        {
            return await _context.Gestores.CountAsync(g => g.SedeId == sedeId);
        }

        public async Task<bool> HasDependenciesAsync(int id)
        {
            return await _context.Sucursais.AnyAsync(s => s.EmpresaId == id) ||
                   await _context.ActividadeEmpresas.AnyAsync(ae => ae.EmpresaId == id) ||
                   await _context.EmpresaBens.AnyAsync(eb => eb.EmpresaId == id) ||
                   await _context.Contactos.AnyAsync(c => c.SedeId == id) ||
                   await _context.Responsaveis.AnyAsync(r => r.SedeId == id) ||
                   await _context.Gestores.AnyAsync(g => g.SedeId == id);
        }
    }
}
