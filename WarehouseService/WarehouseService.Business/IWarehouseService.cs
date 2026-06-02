namespace WarehouseService.Business;

public interface IWarehouseService
{
    Task<bool> CheckAvailabilityAsync(string productId, int quantity);
    Task<bool> ReserveAsync(string productId, int quantity);
}
