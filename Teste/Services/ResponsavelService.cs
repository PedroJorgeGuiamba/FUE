using Teste.Data;
using Teste.Models;
using Microsoft.EntityFrameworkCore;

namespace Teste.Services
{
    public class ResponsavelService
    {
        private readonly FueDbContext _context;

        public ResponsavelService(FueDbContext context)
        {
            _context = context;
        }

        public async Task<List<Responsavel>> GetAllAsync()
        {
            return await _context.Responsaveis.ToListAsync();
        }

        public async Task<Responsavel> GetByIdAsync(int id)
        {
            return await _context.Responsaveis.FindAsync(id);
        }

        public async Task AddAsync(Responsavel responsavel)
        {
            _context.Responsaveis.Add(responsavel);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Responsavel responsavel)
        {
            _context.Entry(responsavel).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Responsaveis.FindAsync(id);
            if (entity != null)
            {
                _context.Responsaveis.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
