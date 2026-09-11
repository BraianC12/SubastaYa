using Application.DTOs;
using Application.UseCases.Billeteras.Commands;

namespace Application.Interfaces
{
    public interface IDepositCommandHandler
    {
        Task<DepositoDto> Handle(DepositCommand request);
    }
}
