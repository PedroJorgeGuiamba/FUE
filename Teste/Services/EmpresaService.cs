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

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Empresas.FindAsync(id);
            if (entity != null)
            {
                _context.Empresas.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
