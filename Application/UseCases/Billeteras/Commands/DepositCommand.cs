using MediatR;

namespace Application.UseCases.Billeteras.Commands
{
    public class DepositCommand : IRequest<decimal>
    {
        public int Usuario_Id { get; set; }
        public decimal Monto { get; set; }
    }
}