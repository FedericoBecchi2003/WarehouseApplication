using Microsoft.AspNetCore.Mvc;
using Order.Business;
using Order.Shared;

namespace Order.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] OrderRequest request)
    {
        try
        {
            var orderId = await _orderService.CreateOrderAsync(request);
            return Ok(new { Message = "Ordine creato con successo", OrderId = orderId });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = ex.Message });
        }
    }
}