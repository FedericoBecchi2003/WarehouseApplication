using PaymentService.Shared;

namespace PaymentService.Business;

public interface IPaymentService
{
    Task<Guid> ProcessPaymentAsync(PaymentRequest request);
    Task<PaymentResponse?> GetPaymentByIdAsync(Guid paymentId);
    Task<bool> RefundAsync(Guid paymentId);
}
