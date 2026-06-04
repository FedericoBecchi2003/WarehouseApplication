using System.Text.Json;
using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OrderService.Repository;
using OrderService.Shared;

namespace OrderService.Business;

public class OrderService : IOrderService
{
    private readonly OrderDbContext _dbContext;
    private readonly ILogger<OrderService> _logger;
    private readonly string _kafkaBootstrapServers;
    private readonly string _topicName;

    public OrderService(OrderDbContext dbContext, IConfiguration configuration, ILogger<OrderService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
        _kafkaBootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092";
        _topicName = configuration["Kafka:TopicName"] ?? "order-events";
    }

    public async Task<Guid> CreateOrderAsync(OrderRequest request)
    {
        var newOrder = new OrderEntity
        {
            Id = Guid.NewGuid(),
            ProductId = request.ProductId,
            Quantity = request.Quantity,
            UserId = request.UserId,
            CreatedAt = DateTime.UtcNow,
            Status = "Pending"
        };

        _dbContext.Orders.Add(newOrder);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Order {OrderId} created for product {ProductId}", newOrder.Id, newOrder.ProductId);

        await PublishKafkaEventAsync(newOrder);

        return newOrder.Id;
    }

    public async Task<OrderResponse?> GetOrderByIdAsync(Guid orderId)
    {
        var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
        if (order is null) return null;

        return new OrderResponse
        {
            Id = order.Id,
            ProductId = order.ProductId,
            Quantity = order.Quantity,
            UserId = order.UserId,
            Status = order.Status,
            CreatedAt = order.CreatedAt
        };
    }

    private async Task PublishKafkaEventAsync(OrderEntity order)
    {
        var config = new ProducerConfig { BootstrapServers = _kafkaBootstrapServers };
        using var producer = new ProducerBuilder<Null, string>(config).Build();

        var eventMessage = new OrderCreatedEvent
        {
            OrderId = order.Id,
            ProductId = order.ProductId,
            Quantity = order.Quantity,
            Timestamp = order.CreatedAt
        };

        var messageString = JsonSerializer.Serialize(eventMessage);
        await producer.ProduceAsync(_topicName, new Message<Null, string> { Value = messageString });

        _logger.LogInformation("Order event published to topic {Topic}", _topicName);
    }
}
