namespace PaymentService.Repository;

public class PaymentEntity
{
    public Guid Id { get; set; }
    public string OrderId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; }
}
