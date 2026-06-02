namespace Order.Shared;

public class OrderRequest
{
    public string ProductId { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string UserId { get; set; } = string.Empty;
}