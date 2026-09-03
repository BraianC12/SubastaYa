using Domain;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IBilleteraRepository
    {
        Task<Billetera> GetByUsuarioIdAsync(int usuarioId);
    }
}