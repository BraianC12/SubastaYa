using Application.Interfaces;
using Microsoft.AspNetCore.SignalR;
using SubastaYa.Hubs;

namespace SubastaYa.Services
{
    public class NotificadorSubastaService : INotificadorSubastaService
    {
        private readonly IHubContext<SubastaHub> _hubContext;

        public NotificadorSubastaService(IHubContext<SubastaHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotificarSubastaIniciadaAsync(int subastaId, string mensaje)
        {
            var data = new
            {
                SubastaId = subastaId,
                Estado = "ACTIVA",
                Mensaje = mensaje
            };

            await _hubContext.Clients.Group($"Subasta_{subastaId}").SendAsync("SubastaIniciada", data);
            await _hubContext.Clients.All.SendAsync("EstadoSubastaCambiado", data);
        }

        public async Task NotificarSubastaFinalizadaAsync(int subastaId, string mensaje, string estadoFinal = "FINALIZADA", int? ganadorId = null, decimal? montoFinal = null)
        {
            var data = new
            {
                SubastaId = subastaId,
                Estado = estadoFinal,
                GanadorId = ganadorId,
                MontoFinal = montoFinal,
                Mensaje = mensaje
            };

            await _hubContext.Clients.Group($"Subasta_{subastaId}").SendAsync("SubastaFinalizada", data);
            await _hubContext.Clients.Group($"Subasta_{subastaId}").SendAsync("SubastaFinaliza", data);
            await _hubContext.Clients.All.SendAsync("EstadoSubastaCambiado", data);
        }

        public async Task NotificarNuevaPujaAsync(int subastaId, decimal nuevoMonto, int compradorId, DateTime fechaFin, bool antiSniping)
        {
            var data = new
            {
                SubastaId = subastaId,
                Monto = nuevoMonto,
                CompradorId = compradorId,
                FechaFin = fechaFin,
                AntiSniping = antiSniping,
                Mensaje = antiSniping
                    ? "¡Tiempo extendido! Se agregaron 2 minutos adicionales por la regla Anti-Sniping."
                    : "Nueva oferta realizada"
            };

            await _hubContext.Clients.Group($"Subasta_{subastaId}").SendAsync("NuevaPuja", data);
            await _hubContext.Clients.All.SendAsync("PujaActualizada", data);
        }
    }
}
