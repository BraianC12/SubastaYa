namespace Application.Interfaces
{
    public interface INotificadorSubastaService
    {
        Task NotificarSubastaIniciadaAsync(int subastaId, string mensaje);
        Task NotificarSubastaFinalizadaAsync(int subastaId, string mensaje, string estadoFinal = "FINALIZADA", int? ganadorId = null, decimal? montoFinal = null);
        Task NotificarNuevaPujaAsync(int subastaId, decimal nuevoMonto, int compradorId, DateTime fechaFin, bool antiSniping);
    }
}
