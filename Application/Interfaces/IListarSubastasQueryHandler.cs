using Application.DTOs;
using Application.UseCases.Subastas.Queries;

namespace Application.Interfaces
{
    public interface IListarSubastasQueryHandler
    {
        Task<IEnumerable<AuctionDto>> Handle(ListarSubastasQuery request);
    }
}
