using Application.DTOs;
using Application.UseCases.Subastas.Commands;

namespace Application.Interfaces
{
    public interface ICreateBidCommandHandler
    {
        Task<CreateBidDto> Handle(CreateBidCommand request);
    }
}
