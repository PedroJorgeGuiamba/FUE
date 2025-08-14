using Teste.Data;
using Teste.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Teste.Services
{
    public class ActividadeService
    {
        private readonly FueDbContext _context;

        public ActividadeService(FueDbContext context)
        {
            _context = context;
        }

        public async Task<List<Actividade>> GetAllAsync()
        {
            return await _context.Actividades.ToListAsync();
        }

        public async Task<Actividade> GetByIdAsync(int id)
        {
            return await _context.Actividades.FindAsync(id);
        }

        public async Task<List<Actividade>> SearchAsync(string termo)
        {
            return await _context.Actividades
                .Where(a => a.Descricao.Contains(termo) || a.CodigoCAE.Contains(termo))
                .Take(10)
                .ToListAsync();
        }

        public async Task AddAsync(Actividade actividade)
        {
            _context.Actividades.Add(actividade);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Actividade actividade)
        {
            _context.Entry(actividade).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Actividades.FindAsync(id);
            if (entity != null)
            {
                _context.Actividades.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
