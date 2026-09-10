using Application.Interfaces;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Persistence.Repositories
{
    public class BilleteraRepository : IBilleteraRepository
    {
        private readonly SubastaDbContext _context;

        public BilleteraRepository(SubastaDbContext context)
        {
            _context = context;
        }

        public async Task<Billetera> GetByUsuarioIdAsync(int usuarioId)
        {
            return await _context.Billeteras
                .Include(b => b.Transacciones)
                .FirstOrDefaultAsync(b => b.Usuario_Id == usuarioId);
        }

        public async Task<List<Billetera>> Listar()
        {
            return await _context.Billeteras
                .Include(b => b.Usuario)
                .ToListAsync();
        }

        public async Task Add(Billetera wallet)
        {
            await _context.Billeteras.AddAsync(wallet);
        }
    }
}