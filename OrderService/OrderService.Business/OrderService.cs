using System.Text.Json;
using Confluent.Kafka;
using OrderService.Repository; // Per leggere il DbContext
using OrderService.Shared;     // Per leggere i DTO

namespace OrderService.Business; // <-- Namespace corretto!

public class OrderService : IOrderService
{
    private readonly OrderDbContext _dbContext;
    private readonly string _kafkaBootstrapServers = "localhost:9092";
    private readonly string _topicName = "order-events";

    public OrderService(OrderDbContext dbContext)
    {
        _dbContext = dbContext;
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
            Status = "Completed"
        };

        _dbContext.Orders.Add(newOrder);
        await _dbContext.SaveChangesAsync();

        await PublishKafkaEventAsync(newOrder);

        return newOrder.Id;
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
    }
}