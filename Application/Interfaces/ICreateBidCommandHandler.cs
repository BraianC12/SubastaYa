using Application.UseCases.Subastas.Commands;

namespace Application.Interfaces
{
    public interface ICreateBidCommandHandler
    {
        Task<int> Handle(CreateBidCommand request);
    }
}
