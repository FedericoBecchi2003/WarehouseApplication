namespace OrderService.Shared;

public class OrderCreatedEvent
{
    public Guid OrderId { get; set; }
    public string ProductId { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public DateTime Timestamp { get; set; }
}