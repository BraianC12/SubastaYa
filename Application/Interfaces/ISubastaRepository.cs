using Domain;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ISubastaRepository
    {
        Task AddAsync(Subasta subasta);
        Task<Subasta> GetByIdAsync(int id);
    }
}