using Application.Interfaces;
using Domain;
using System.Linq;
using System.Threading.Tasks;

namespace Application.UseCases.Subastas.Handlers
{
    public class AdjudicarSubastasCommandHandler : IAdjudicarSubastasCommandHandler
    {
        private readonly ISubastaRepository _subastaRepository;
        private readonly IBilleteraRepository _billeteraRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditorialLogRepository _auditoriaRepository;
        private readonly ITransaccionLedgerRepository _transaccionRepository;

        public AdjudicarSubastasCommandHandler(ISubastaRepository subastaRepository, IBilleteraRepository billeteraRepository, IUnitOfWork unitOfWork, IAuditorialLogRepository auditoriaRepository, ITransaccionLedgerRepository transaccionRepository)
        {
            _subastaRepository = subastaRepository;
            _billeteraRepository = billeteraRepository;
            _unitOfWork = unitOfWork;
            _auditoriaRepository = auditoriaRepository;
            _transaccionRepository = transaccionRepository;
        }

        public async Task Handle()
        {
            var subastasVencidas = await _subastaRepository.ObtenerVencidasActivasAsync();

            if (!subastasVencidas.Any()) return;

            foreach (var subasta in subastasVencidas)
            {
                if (subasta.Pujas == null || !subasta.Pujas.Any())
                {
                    //si no tiene pujas, pasa a DESIERTA
                    subasta.Estado = "DESIERTA";

                    var logDesierta = new Auditoria_Log
                    {
                        Entidad = "Subasta",
                        Entidad_Id = subasta.Id,
                        Accion = "PASO_A_DESIERTA",
                        Detalle = "La subasta vencio sin recibir ninguna puja",
                        Fecha = DateTime.UtcNow,
                        Usuario_Id = null
                    };
                    await _auditoriaRepository.AddAsync(logDesierta);
                }
                else
                {
                    //Si tiene pujas, pasa a FINALIZADA
                    subasta.Estado = "FINALIZADA";

                    var pujaGanadora = subasta.Pujas.OrderByDescending(p => p.Monto).First();

                    var billeteraComprador = await _billeteraRepository.GetByUsuarioIdAsync(pujaGanadora.Comprador_Id);
                    var billeteraVendedor = await _billeteraRepository.GetByUsuarioIdAsync(subasta.Vendedor_Id);

                    if (billeteraComprador != null && billeteraVendedor != null)
                    {
                        //debitar al comprador
                        billeteraComprador.Saldo_Total -= pujaGanadora.Monto;

                        //acreditar al vendedor
                        billeteraVendedor.Saldo_Total += pujaGanadora.Monto;
                        billeteraVendedor.Saldo_Disponible += pujaGanadora.Monto;
                    }


                    var transaccion = new Transaccion_Ledger
                    {
                        Tipo = "VENTA",
                        Monto = pujaGanadora.Monto,
                        Fecha = DateTime.UtcNow,
                        Subasta_Id = subasta.Id,
                        Billetera_Id = billeteraComprador.Id
                    };

                    await _transaccionRepository.AddAsync(transaccion);

                    
                    var logVenta = new Auditoria_Log
                    {
                        Entidad = "Subasta",
                        Entidad_Id = subasta.Id,
                        Accion = "VENTA_CONCRETA",
                        Detalle = $"Subasta liquidada. Comprador ID: {pujaGanadora.Comprador_Id}, Vendedor ID: {subasta.Vendedor_Id}, Monto final: ${pujaGanadora.Monto}.",
                        Fecha = DateTime.UtcNow,
                        Usuario_Id = null
                    };
                    await _auditoriaRepository.AddAsync(logVenta);
                }
            }
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
