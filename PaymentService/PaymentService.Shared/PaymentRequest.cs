using System.ComponentModel.DataAnnotations;

namespace PaymentService.Shared;

public class PaymentRequest
{
    [Required] public string OrderId { get; set; } = string.Empty;
    [Range(0.01, double.MaxValue)] public decimal Amount { get; set; }
    [Required] public string UserId { get; set; } = string.Empty;
}
