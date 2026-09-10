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

        public async Task NotificarSubastaFinalizadaAsync(int subastaId, string mensaje)
        {
            await _hubContext.Clients.Group($"Subasta_{subastaId}").SendAsync("SubastaFinaliza", new
            {
                SubastaId = subastaId,
                Mensaje = mensaje
            });
        }
    }
}
