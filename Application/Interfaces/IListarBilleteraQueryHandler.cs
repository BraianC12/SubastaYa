
using Application.DTOs;
using Application.UseCases.Billeteras.Queries;

namespace Application.Interfaces
{
    public interface IListarBilleteraQueryHandler
    {
        Task<WalletBalanceDto> Handle(ListarBilleteraQuery request);
    }
}
