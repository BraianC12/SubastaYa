using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Subastas.Commands;
using Domain;
using Domain.Exceptions;


namespace Application.UseCases.Subastas.Handlers
{
    public class CreateBidCommandHandler: ICreateBidCommandHandler
    {
        private readonly ISubastaRepository _subastaRepository;
        private readonly IBilleteraRepository _billeteraRepository;
        private readonly IAuditorialLogRepository _auditoriaRepository;
        private ITransaccionLedgerRepository _transaccionLedgerRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateBidCommandHandler(
            ISubastaRepository subastaRepository,
            IBilleteraRepository billeteraRepository,
            IAuditorialLogRepository auditoriaRepository,
            ITransaccionLedgerRepository transaccionLedgerRepository,
            IUnitOfWork unitOfWork)
        {
            _subastaRepository = subastaRepository;
            _billeteraRepository = billeteraRepository;
            _auditoriaRepository = auditoriaRepository;
            _transaccionLedgerRepository = transaccionLedgerRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateBidDto> Handle(CreateBidCommand request)
        {
            // Validaciones de la Subasta
            var subasta = await _subastaRepository.GetByIdAsync(request.Subasta_Id);
            if (subasta == null)
                throw new NotFoundException("Subasta no encontrada.");

            if (subasta.Vendedor_Id == request.Comprador_Id)
                throw new DomainException("El vendedor no puede pujar en su propia subasta.");

            if (subasta.Estado.ToUpper() != "ACTIVA")
                throw new ConflictException("La subasta ya finalizó o no está activa.");

            // validaciones de la Billetera del nuevo comprador
            var billeteraComprador = await _billeteraRepository.GetByUsuarioIdAsync(request.Comprador_Id);
            if (billeteraComprador == null)
                throw new NotFoundException("Billetera no encontrada.");

            if (billeteraComprador.Saldo_Disponible < request.Monto)
                throw new DomainException("El saldo disponible es menor al monto seleccionado.");

            // valida el monto mínimo de la puja
            var pujaActual = subasta.Pujas.OrderByDescending(p => p.Monto).FirstOrDefault();
            decimal montoMinimo = pujaActual == null? 
                subasta.Precio_Base: pujaActual.Monto + subasta.Incremento_Minimo;

            if (request.Monto < montoMinimo)
                throw new DomainException($"El monto mínimo para pujar debe ser {montoMinimo}.");

            // retiene el dinero del nuevo comprador
            billeteraComprador.Saldo_Disponible -= request.Monto;
            billeteraComprador.Saldo_Retenido += request.Monto;

            var transaccionRetencion = new Transaccion_Ledger
            {
                Tipo = "RETENCION",
                Monto = request.Monto,
                Fecha = DateTime.UtcNow,
                Subasta_Id = subasta.Id,
                Billetera_Id = billeteraComprador.Id
            };

            await _transaccionLedgerRepository.AddAsync(transaccionRetencion);
            
            if (pujaActual != null)
            {
                var billeteraAnterior = await _billeteraRepository.GetByUsuarioIdAsync(pujaActual.Comprador_Id);
                if (billeteraAnterior != null)
                {
                    billeteraAnterior.Saldo_Retenido -= pujaActual.Monto;
                    billeteraAnterior.Saldo_Disponible += pujaActual.Monto;

                    var transaccionLiberacion = new Transaccion_Ledger
                    {
                        Tipo = "LIBERACION",
                        Monto = pujaActual.Monto,
                        Fecha = DateTime.UtcNow,
                        Subasta_Id = subasta.Id,
                        Billetera_Id = billeteraAnterior.Id
                    };

                    await _transaccionLedgerRepository.AddAsync(transaccionLiberacion);
                }
            }

            // extiende el tiempo si quedan menos de 1 minuto
            var tiempoRestante = subasta.Fecha_Fin - DateTime.UtcNow;
            if (tiempoRestante <= TimeSpan.FromMinutes(1))
            {
                subasta.Fecha_Fin = subasta.Fecha_Fin.AddMinutes(2);

                var logExtension = new Auditoria_Log
                {
                    Entidad = "Subasta",
                    Entidad_Id = subasta.Id,
                    Accion = "EXTENSION DE TIEMPO",
                    Detalle = "El tiempo de la subasta se extendio 2 minutos por la regla Anti-Sniping.",
                    Fecha = DateTime.UtcNow,
                    Usuario_Id = request.Comprador_Id
                };

                await _auditoriaRepository.AddAsync(logExtension);
            }

            //Crear la nueva puja
            var nuevaPuja = new Puja
            {
                Monto = request.Monto,
                Fecha_Puja = DateTime.UtcNow,
                Comprador_Id = request.Comprador_Id
            };
            subasta.Pujas.Add(nuevaPuja);

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (ConflictException)
            {
                _unitOfWork.Clear();

                var logConcurrencia = new Auditoria_Log
                {
                    Entidad = "Subasta",
                    Entidad_Id = subasta.Id,
                    Accion = "PUJA_RECHAZADA_CONCURRENCIA",
                    Detalle = "Intento de puja rechazada por colisión de concurrencia.",
                    Fecha = DateTime.UtcNow,
                    Usuario_Id = request.Comprador_Id
                };

                await _auditoriaRepository.AddAsync(logConcurrencia);

                await _unitOfWork.SaveChangesAsync();

                throw new ConflictException("Otro usuario realizó una puja simultáneamente. Intente pujar con el nuevo valor.");
            }

            return new CreateBidDto
            {
                PujaId = nuevaPuja.Id,
                SubastaId = subasta.Id,
                Monto = nuevaPuja.Monto,
                Fecha = nuevaPuja.Fecha_Puja,
                SaldoDisponibleRestante = billeteraComprador.Saldo_Disponible
            };
        }
    }
}