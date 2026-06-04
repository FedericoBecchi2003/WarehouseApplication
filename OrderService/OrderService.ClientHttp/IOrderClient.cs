using OrderService.Shared;
using Refit;

namespace OrderService.ClientHttp;

public interface IOrderClient
{
    [Get("/api/orders/{orderId}")]
    Task<OrderResponse> GetOrderByIdAsync(Guid orderId);

    [Post("/api/orders")]
    Task<Guid> CreateOrderAsync([Body] OrderRequest request);
}
