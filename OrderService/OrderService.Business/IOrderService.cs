using OrderService.Shared;

namespace OrderService.Business;

public interface IOrderService
{
    Task<Guid> CreateOrderAsync(OrderRequest request);
    Task<OrderResponse?> GetOrderByIdAsync(Guid orderId);
}
