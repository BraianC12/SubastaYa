using Application.DTOs;
using Application.UseCases.Handlers;
using Application.UseCases.Subastas.Queries;

namespace Application.Interfaces
{
    public interface IObtenerSubastaQueryHandler
    {
        Task<AuctionDetailDto> Handle(ObtenerSubastaQuery request);
    }
}
