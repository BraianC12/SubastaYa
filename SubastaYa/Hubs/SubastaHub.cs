using Microsoft.AspNetCore.SignalR;


namespace SubastaYa.Hubs
{
    public class SubastaHub : Hub
    {
        //para que el cliente se una a una sala específica
        public async Task UnirseSubasta(string subastaId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Subasta_{subastaId}");
        }

        //salir de la sala si el usuario cambia de pantalla
        public async Task SalirDeSubasta(string subastaId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Subasta_{subastaId}");
        }
    }
}
