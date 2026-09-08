using Application.Interfaces;
using Domain;
using Infraestructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class AuditoriaLogRepository : IAuditorialLogRepository
    {
        private readonly SubastaDbContext _context;

        public AuditoriaLogRepository(SubastaDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Auditoria_Log auditoria)
        {
            await _context.Set<Auditoria_Log>().AddAsync(auditoria);
        }
    }
}
