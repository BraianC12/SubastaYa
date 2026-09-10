using Application.Interfaces;
using Application.UseCases.Billeteras.Commands;
using Domain;
using Domain.Exceptions;


namespace Application.UseCases.Billeteras.Handlers
{
    public class DepositCommandHandler: IDepositCommandHandler
    {
        private readonly IBilleteraRepository _billeteraRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DepositCommandHandler(IBilleteraRepository billeteraRepository, IUnitOfWork unitOfWork)
        {
            _billeteraRepository = billeteraRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<decimal> Handle(DepositCommand request)
        {
            //Valida que el monto sea positivo
            if (request.Monto <= 0)
                throw new DomainException("El monto a depositar debe ser mayor a cero.");

            //buscar la billetera del usuario
            var billetera = await _billeteraRepository.GetByUsuarioIdAsync(request.Usuario_Id);
            if (billetera == null)
                throw new NotFoundException("Billetera no encontrada para el usuario especificado.");

            //actualiza los datos
            billetera.Saldo_Total += request.Monto;
            billetera.Saldo_Disponible += request.Monto;

           
            billetera.Transacciones.Add(new Transaccion_Ledger
            {
                Tipo = "DEPOSITO",
                Monto = request.Monto,
                Fecha = DateTime.UtcNow,
                Billetera_Id = billetera.Id
            });
           
            await _unitOfWork.SaveChangesAsync();

            return billetera.Saldo_Total;
        }
    }
}