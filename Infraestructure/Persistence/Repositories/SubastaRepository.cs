using Application.Interfaces;
using Domain;
using Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Persistence.Repositories
{
    public class SubastaRepository : ISubastaRepository
    {
        private readonly SubastaDbContext _context;

        public SubastaRepository(SubastaDbContext context)
        {
            _context = context;
        }

        public async Task<List<Subasta>> Listar()
        {
            return await _context.Subastas.Include(s => s.Categoria).Include(s => s.Pujas).ToListAsync();
        }

        public async Task<Subasta> Obtener(int id)
        {
            return await _context.Subastas
                .Include(s => s.Pujas)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task AddAsync(Subasta subasta)
        {
            await _context.Subastas.AddAsync(subasta);
        }

        public async Task<Subasta> GetByIdAsync(int id)
        {
            return await _context.Subastas
                .Include(s => s.Pujas)
                .Include(s => s.Transacciones)
                .FirstOrDefaultAsync(s => s.Id == id);
        }
    }
}