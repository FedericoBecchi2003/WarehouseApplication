using PaymentService.Shared;
using Refit;

namespace PaymentService.ClientHttp;

public interface IPaymentClient
{
    [Get("/api/payments/{paymentId}")]
    Task<PaymentResponse> GetPaymentByIdAsync(Guid paymentId);

    [Post("/api/payments")]
    Task<Guid> ProcessPaymentAsync([Body] PaymentRequest request);

    [Post("/api/payments/{paymentId}/refund")]
    Task<bool> RefundAsync(Guid paymentId);
}
