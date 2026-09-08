using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Billeteras.Queries;
using Domain.Exceptions;


namespace Application.UseCases.Billeteras.Handlers
{
    public class ListarBilleteraQueryHandler: IListarBilleteraQueryHandler
    {
        private readonly IBilleteraRepository _repository;

        public ListarBilleteraQueryHandler(IBilleteraRepository billeteraRepository)
        {
            _repository = billeteraRepository;
        }

        public async Task<WalletBalanceDto> Handle(ListarBilleteraQuery request)
        {
            var billetera = await _repository.GetByUsuarioIdAsync(request.Usuario_Id);

            if (billetera == null)
                throw new NotFoundException("Billetera no encontrada.");

            return new WalletBalanceDto
            {
                Usuario_Id = billetera.Usuario_Id,
                Saldo_Total = billetera.Saldo_Total,
                Saldo_Retenido = billetera.Saldo_Retenido,
                Saldo_Disponible = billetera.Saldo_Disponible
            };
        }
    }
}