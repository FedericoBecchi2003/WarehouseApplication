using Microsoft.AspNetCore.Mvc;
using OrderService.Business;
using OrderService.Shared;

namespace OrderService.WebApi.Controllers;

[ApiController]
[Route("api/orders")]
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
        var orderId = await _orderService.CreateOrderAsync(request);
        return CreatedAtAction(nameof(GetOrderById), new { orderId }, new { OrderId = orderId });
    }

    [HttpGet("{orderId:guid}")]
    public async Task<IActionResult> GetOrderById(Guid orderId)
    {
        var order = await _orderService.GetOrderByIdAsync(orderId);
        if (order is null) return NotFound();
        return Ok(order);
    }
}
