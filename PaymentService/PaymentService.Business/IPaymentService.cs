namespace PaymentService.Business;

public interface IPaymentService
{
    Task<bool> ProcessPaymentAsync(string orderId, decimal amount);
    Task<bool> RefundAsync(string orderId, decimal amount);
}
