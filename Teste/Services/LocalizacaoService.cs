using Teste.Data;
using Teste.Models;
using Microsoft.EntityFrameworkCore;

namespace Teste.Services
{
    public class LocalizacaoService
    {
        private readonly FueDbContext _context;

        public LocalizacaoService(FueDbContext context)
        {
            _context = context;
        }

        public async Task<List<Localizacao>> GetAllAsync()
        {
            return await _context.Localizacoes.ToListAsync();
        }

        public async Task<Localizacao> GetByIdAsync(int id)
        {
            return await _context.Localizacoes.FindAsync(id);
        }

        public async Task AddAsync(Localizacao localizacao)
        {
            _context.Localizacoes.Add(localizacao);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Localizacao localizacao)
        {
            _context.Entry(localizacao).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Localizacoes.FindAsync(id);
            if (entity != null)
            {
                _context.Localizacoes.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
