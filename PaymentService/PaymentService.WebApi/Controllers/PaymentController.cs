using Microsoft.AspNetCore.Mvc;
using PaymentService.Business;
using PaymentService.Shared;

namespace PaymentService.WebApi.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost]
    public async Task<IActionResult> ProcessPayment([FromBody] PaymentRequest request)
    {
        var paymentId = await _paymentService.ProcessPaymentAsync(request);
        return Ok(paymentId);
    }

    [HttpGet("{paymentId:guid}")]
    public async Task<IActionResult> GetPaymentById(Guid paymentId)
    {
        var payment = await _paymentService.GetPaymentByIdAsync(paymentId);
        if (payment is null) return NotFound();
        return Ok(payment);
    }

    [HttpPost("{paymentId:guid}/refund")]
    public async Task<IActionResult> Refund(Guid paymentId)
    {
        var result = await _paymentService.RefundAsync(paymentId);
        if (!result) return NotFound();
        return Ok();
    }
}
