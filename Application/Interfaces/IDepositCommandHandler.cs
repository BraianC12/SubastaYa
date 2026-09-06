using Application.UseCases.Billeteras.Commands;

namespace Application.Interfaces
{
    public interface IDepositCommandHandler
    {
        Task<decimal> Handle(DepositCommand request);
    }
}
