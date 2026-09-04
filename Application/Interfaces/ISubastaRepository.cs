using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ISubastaRepository
    {
        Task<Subasta> Obtener(int id);
        Task<List<Subasta>> Listar();
        Task Agregar(Subasta entidad);
        void Eliminar(Subasta entidad);
        Task AddAsync(Subasta subasta);
        Task<Subasta> GetByIdAsync(int id);
    }
}
      
