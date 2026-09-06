using Application.UseCases.Subastas.Commands;

namespace Application.Interfaces
{
    public interface ICreateSubastaCommandHandler
    {
        Task<int> Handle(CreateSubastaCommand request);
    }
}
