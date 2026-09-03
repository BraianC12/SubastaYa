using Application.Interfaces;
using Domain;
using Infraestructure.Persistence;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class SubastaRepository : ISubastaRepository
    {
        private readonly SubastaDbContext _context;

        public SubastaRepository(SubastaDbContext context)
        {
            _context = context;
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