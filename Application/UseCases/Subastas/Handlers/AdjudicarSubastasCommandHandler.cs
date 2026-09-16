using Application.Interfaces;
using Domain;


namespace Application.UseCases.Subastas.Handlers
{
    public class AdjudicarSubastasCommandHandler : IAdjudicarSubastasCommandHandler
    {
        private readonly ISubastaRepository _subastaRepository;
        private readonly IBilleteraRepository _billeteraRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditorialLogRepository _auditoriaRepository;
        private readonly ITransaccionLedgerRepository _transaccionRepository;
        private readonly INotificadorSubastaService _notificador;

        public AdjudicarSubastasCommandHandler(ISubastaRepository subastaRepository, IBilleteraRepository billeteraRepository, IUnitOfWork unitOfWork, IAuditorialLogRepository auditoriaRepository, ITransaccionLedgerRepository transaccionRepository, INotificadorSubastaService notificador)
        {
            _subastaRepository = subastaRepository;
            _billeteraRepository = billeteraRepository;
            _unitOfWork = unitOfWork;
            _auditoriaRepository = auditoriaRepository;
            _transaccionRepository = transaccionRepository;
            _notificador = notificador;
        }
        public async Task Handle()
        {
            var subastasProgramadas = await _subastaRepository.GetByEstadoAsync("PROGRAMADA");
            var subastasVencidas = await _subastaRepository.ObtenerVencidasActivasAsync();

            // 1. Procesar las programadas que deban activarse
            foreach (var subasta in subastasProgramadas)
            {
                if (DateTime.Now >= subasta.Fecha_Inicio)
                {
                    subasta.Estado = "ACTIVA";

                    var logActivacion = new Auditoria_Log
                    {
                        Entidad = "Subasta",
                        Entidad_Id = subasta.Id,
                        Accion = "INICIO_DE_SUBASTA",
                        Detalle = "La subasta programada comenzó automáticamente",
                        Fecha = DateTime.Now,
                        Usuario_Id = null
                    };
                    await _auditoriaRepository.AddAsync(logActivacion);
                }
            }

            // 2. Procesar las activas que ya vencieron
            foreach (var subasta in subastasVencidas)
            {
                if (DateTime.Now > subasta.Fecha_Fin)
                {
                    if (subasta.Pujas == null || !subasta.Pujas.Any())
                    {
                        subasta.Estado = "DESIERTA";
                        var logDesierta = new Auditoria_Log
                        {
                            Entidad = "Subasta",
                            Entidad_Id = subasta.Id,
                            Accion = "PASO_A_DESIERTA",
                            Detalle = "La subasta vencio sin recibir ninguna puja",
                            Fecha = DateTime.Now,
                            Usuario_Id = subasta.Vendedor_Id
                        };
                        await _auditoriaRepository.AddAsync(logDesierta);
                    }
                    else
                    {
                        subasta.Estado = "FINALIZADA";
                        var pujaGanadora = subasta.Pujas.OrderByDescending(p => p.Monto).FirstOrDefault();

                        if (pujaGanadora != null)
                        {
                            var billeteraComprador = await _billeteraRepository.GetByUsuarioIdAsync(pujaGanadora.Comprador_Id);
                            var billeteraVendedor = await _billeteraRepository.GetByUsuarioIdAsync(subasta.Vendedor_Id);

                            if (billeteraComprador != null && billeteraVendedor != null)
                            {
                                billeteraComprador.Saldo_Total -= pujaGanadora.Monto;
                                billeteraComprador.Saldo_Retenido -= pujaGanadora.Monto;

                                billeteraVendedor.Saldo_Total += pujaGanadora.Monto;
                                billeteraVendedor.Saldo_Disponible += pujaGanadora.Monto;

                                var transaccionDebito = new Transaccion_Ledger
                                {
                                    Tipo = "DEBITO",
                                    Monto = pujaGanadora.Monto,
                                    Fecha = DateTime.Now,
                                    Subasta_Id = subasta.Id,
                                    Billetera_Id = billeteraComprador.Id
                                };
                                await _transaccionRepository.AddAsync(transaccionDebito);

                                var transaccionDeposito = new Transaccion_Ledger
                                {
                                    Tipo = "DEPOSITO",
                                    Monto = pujaGanadora.Monto,
                                    Fecha = DateTime.Now,
                                    Subasta_Id = subasta.Id,
                                    Billetera_Id = billeteraVendedor.Id
                                };
                                await _transaccionRepository.AddAsync(transaccionDeposito);
                            }

                            var logVenta = new Auditoria_Log
                            {
                                Entidad = "Subasta",
                                Entidad_Id = subasta.Id,
                                Accion = "VENTA_CONCRETA",
                                Detalle = $"Subasta liquidada. Comprador ID: {pujaGanadora.Comprador_Id}, Vendedor ID: {subasta.Vendedor_Id}, Monto final: ${pujaGanadora.Monto}.",
                                Fecha = DateTime.Now,
                                Usuario_Id = null
                            };
                            await _auditoriaRepository.AddAsync(logVenta);
                            await _notificador.NotificarSubastaFinalizadaAsync(subasta.Id, "La subasta ha finalizado");
                        }
                    }
                }
            }

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
