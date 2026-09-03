using Application.Interfaces;
using Domain;
using Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

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
                .FirstOrDefaultAsync(b => b.Usuario_Id == usuarioId);
        }
    }
}