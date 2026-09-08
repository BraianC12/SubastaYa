using Application.Interfaces;
using Domain;
using Infraestructure.Persistence;
using Infrastructure.Persistence;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class TransaccionLedgerRepository : ITransaccionLedgerRepository
    {
        private readonly SubastaDbContext _context;

        public TransaccionLedgerRepository(SubastaDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Transaccion_Ledger transaccion)
        {
            await _context.Set<Transaccion_Ledger>().AddAsync(transaccion);
        }
    }
}