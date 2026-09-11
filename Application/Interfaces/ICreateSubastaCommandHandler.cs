using Application.DTOs;
using Application.UseCases.Subastas.Commands;

namespace Application.Interfaces
{
    public interface ICreateSubastaCommandHandler
    {
        Task<CreateAuctionDto> Handle(CreateSubastaCommand request);
    }
}
