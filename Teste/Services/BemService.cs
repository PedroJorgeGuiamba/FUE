using Teste.Data;
using Teste.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Teste.Services
{
    public class BemService
    {
        private readonly FueDbContext _context;

        public BemService(FueDbContext context)
        {
            _context = context;
        }

        public async Task<List<Bem>> GetAllAsync()
        {
            return await _context.Bens.ToListAsync();
        }

        public async Task<Bem> GetByIdAsync(int id)
        {
            return await _context.Bens.FindAsync(id);
        }

        public async Task<List<Bem>> SearchAsync(string termo)
        {
            return await _context.Bens
                .Where(a => a.Descricao.Contains(termo) || a.CodigoCNBS.Contains(termo))
                .Take(10)
                .ToListAsync();
        }

        public async Task AddAsync(Bem bem)
        {
            _context.Bens.Add(bem);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Bem bem)
        {
            _context.Entry(bem).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Bens.FindAsync(id);
            if (entity != null)
            {
                _context.Bens.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
