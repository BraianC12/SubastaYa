namespace Application.Interfaces
{
    public interface INotificadorSubastaService
    {
        Task NotificarSubastaFinalizadaAsync(int subastaId, string mensaje);
    }
}
