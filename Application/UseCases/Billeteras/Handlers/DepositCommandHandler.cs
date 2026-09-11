using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Billeteras.Commands;
using Domain;
using Domain.Exceptions;


namespace Application.UseCases.Billeteras.Handlers
{
    public class DepositCommandHandler: IDepositCommandHandler
    {
        private readonly IBilleteraRepository _billeteraRepository;
        private readonly ITransaccionLedgerRepository _transaccionLedgerRepository;
        private readonly IAuditorialLogRepository _auditorialLogRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DepositCommandHandler(IBilleteraRepository billeteraRepository, ITransaccionLedgerRepository transaccionLedgerRepository, IAuditorialLogRepository auditorialLogRepository, IUnitOfWork unitOfWork)
        {
            _billeteraRepository = billeteraRepository;
            _transaccionLedgerRepository = transaccionLedgerRepository;
            _auditorialLogRepository = auditorialLogRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<DepositoDto> Handle(DepositCommand request)
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

            var  transaccion = new Transaccion_Ledger
            {
                Tipo = "DEPOSITO",
                Monto = request.Monto,
                Fecha = DateTime.UtcNow,
                Billetera_Id = billetera.Id
            };

            await _transaccionLedgerRepository.AddAsync(transaccion);

            var logDeposito = new Auditoria_Log
            {
                Entidad = "Billetera",
                Entidad_Id = billetera.Id,
                Accion = "DEPOSITO_SALDO",
                Detalle = $"Depósito de ${request.Monto}. Saldo total: ${billetera.Saldo_Total}.",
                Fecha = DateTime.UtcNow,
                Usuario_Id = request.Usuario_Id
            };

            await _auditorialLogRepository.AddAsync(logDeposito);
           
            await _unitOfWork.SaveChangesAsync();

            return new DepositoDto
            {
                TransaccionId = transaccion.Id,
                MontoDepositado = request.Monto,
                SaldoTotal = billetera.Saldo_Total,
                SaldoDisponible = billetera.Saldo_Disponible,
                Fecha = transaccion.Fecha
            };
        }
    }
}