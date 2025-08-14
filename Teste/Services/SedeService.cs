using Teste.Data;
using Teste.Models;
using Microsoft.EntityFrameworkCore;

namespace Teste.Services
{
    public class SedeService
    {
        private readonly FueDbContext _context;

        public SedeService(FueDbContext context)
        {
            _context = context;
        }

        public async Task<List<Sede>> GetAllAsync()
        {
            return await _context.Sedes.ToListAsync();
        }

        public async Task<Sede> GetByIdAsync(int id)
        {
            return await _context.Sedes.FindAsync(id);
        }

        public async Task AddAsync(Sede sede)
        {
            _context.Sedes.Add(sede);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Sede sede)
        {
            _context.Entry(sede).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Sedes.FindAsync(id);
            if (entity != null)
            {
                _context.Sedes.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
