using Teste.Data;
using Teste.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Teste.Services
{
    public class ContactoService
    {
        private readonly FueDbContext _context;

        public ContactoService(FueDbContext context)
        {
            _context = context;
        }

        public async Task<List<Contacto>> GetAllAsync()
        {
            return await _context.Contactos.ToListAsync();
        }

        public async Task<Contacto> GetByIdAsync(int id)
        {
            return await _context.Contactos.FindAsync(id);
        }

        public async Task AddAsync(Contacto contacto)
        {
            _context.Contactos.Add(contacto);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Contacto contacto)
        {
            _context.Entry(contacto).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Contactos.FindAsync(id);
            if (entity != null)
            {
                _context.Contactos.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
