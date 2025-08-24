using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Teste.Data;
using Teste.Models;

namespace Teste.Services
{
    public class SucursalService
    {
        private readonly FueDbContext _context;

        public SucursalService(FueDbContext context)
        {
            _context = context;
        }

        public async Task<List<Sucursal>> GetAllAsync()
        {
            return await _context.Sucursais
                .ToListAsync();
        }

        public async Task<Sucursal> GetByIdAsync(int id)
        {
            return await _context.Sucursais.FindAsync(id);
        }

        public async Task AddAsync(Sucursal sucursal)
        {
            _context.Sucursais.Add(sucursal);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Sucursal sucursal)
        {
            _context.Entry(sucursal).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Sucursais.FindAsync(id);
            if (entity != null)
            {
                _context.Sucursais.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
