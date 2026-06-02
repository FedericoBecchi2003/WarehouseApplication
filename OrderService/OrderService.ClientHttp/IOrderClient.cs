using OrderService.Shared;
using Refit;

namespace OrderService.ClientHttp;

public interface IOrderClient
{
    // Questo attributo dice a Refit: "Quando chiami questo metodo, fai una GET a questo URL"
    [Get("/api/order/{orderId}")]
    Task<OrderResponse> GetOrderByIdAsync(Guid orderId);

    // Esempio di una POST
    [Post("/api/order")]
    Task<Guid> CreateOrderAsync([Body] OrderRequest request);
}