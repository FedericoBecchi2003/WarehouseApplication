namespace PaymentService.Shared;

public class PaymentProcessedEvent
{
    public Guid PaymentId { get; set; }
    public string OrderId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
