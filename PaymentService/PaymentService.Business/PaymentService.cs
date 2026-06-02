namespace PaymentService.Business;

public class PaymentService : IPaymentService
{
    public async Task<bool> ProcessPaymentAsync(string orderId, decimal amount)
    {
        // TODO: Implementare logica di elaborazione pagamento
        await Task.Delay(10);
        return true;
    }

    public async Task<bool> RefundAsync(string orderId, decimal amount)
    {
        // TODO: Implementare logica di rimborso
        await Task.Delay(10);
        return true;
    }
}
