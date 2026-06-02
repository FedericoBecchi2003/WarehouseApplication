namespace WarehouseService.Business;

public class WarehouseService : IWarehouseService
{
    public async Task<bool> CheckAvailabilityAsync(string productId, int quantity)
    {
        // TODO: Implementare logica di controllo disponibilità
        await Task.Delay(10);
        return true;
    }

    public async Task<bool> ReserveAsync(string productId, int quantity)
    {
        // TODO: Implementare logica di prenotazione
        await Task.Delay(10);
        return true;
    }
}
