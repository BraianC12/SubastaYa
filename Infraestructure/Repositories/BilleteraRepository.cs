using Application.Interfaces;
using Domain;
using Infraestructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructure.Repositories
{
    public class BilleteraRepository : IBilleteraRepository
    {
        protected readonly SubastaDbContext _context;

        public BilleteraRepository(SubastaDbContext context)
        {
            _context = context;
        }

        public async Task<List<Billetera>> Listar()
        {
            var billeteras = await _context.Billeteras.Include(b => b.Usuario).ToListAsync();

            return billeteras;
        }
    }
}
