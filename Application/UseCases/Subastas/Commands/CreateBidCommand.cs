using MediatR;

namespace Application.UseCases.Subastas.Commands
{
    public class CreateBidCommand : IRequest<int>
    {
        public int Subasta_Id { get; set; }
        public int Comprador_Id { get; set; }
        public decimal Monto { get; set; }
    }
}