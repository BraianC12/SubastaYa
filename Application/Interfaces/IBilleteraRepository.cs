using Domain;

namespace Application.Interfaces
{
    public interface IBilleteraRepository
    {
        Task<Billetera> GetByUsuarioIdAsync(int usuarioId);
    }
}