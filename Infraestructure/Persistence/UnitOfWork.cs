using Application.Interfaces;
using Domain.Exceptions;
using Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SubastaDbContext _context;

        public UnitOfWork(SubastaDbContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new ConflictException("Conflicto de concurrencia: el recurso fue modificado por otra transacción simultánea. Por favor, recargue e intente nuevamente.");
            }
        }

        public void Clear()
        {
            _context.ChangeTracker.Clear();
        }
    }
}