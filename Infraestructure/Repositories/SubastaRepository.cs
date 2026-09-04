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
    public class SubastaRepository : ISubastaRepository
    {
        protected readonly SubastaDbContext _context;

        public SubastaRepository(SubastaDbContext context)
        {
            _context = context;
        }

        public Task Agregar(Subasta entidad)
        {
            throw new NotImplementedException();
        }

        public void Eliminar(Subasta entidad)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Subasta>> Listar()
        {
            var subastas = await _context.Subastas.ToListAsync();

            return subastas;
        }

        public async Task<Subasta> Obtener(int id)
        {
            var subasta = await _context.Subastas.Include(s => s.Pujas).FirstOrDefaultAsync(s => s.Id == id);

            return subasta;
        }
    }
}
