using Application.DTOs;
using MediatR;

namespace Application.UseCases.Billeteras.Queries
{
    public class ListarBilleteraQuery : IRequest<WalletBalanceDto>
    {
        public int Usuario_Id { get; set; }
    }
}