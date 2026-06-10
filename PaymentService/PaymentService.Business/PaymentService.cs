using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PaymentService.Repository;
using PaymentService.Shared;

namespace PaymentService.Business;

public class PaymentService : IPaymentService
{
    private readonly PaymentDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(PaymentDbContext context, IConfiguration configuration, ILogger<PaymentService> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<Guid> ProcessPaymentAsync(PaymentRequest request)
    {
        var payment = new PaymentEntity
        {
            Id = Guid.NewGuid(),
            OrderId = request.OrderId,
            Amount = request.Amount,
            UserId = request.UserId,
            Status = "Completed",
            CreatedAt = DateTime.UtcNow
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        await PublishKafkaEventAsync(new PaymentProcessedEvent
        {
            PaymentId = payment.Id,
            OrderId = payment.OrderId,
            Amount = payment.Amount,
            Status = payment.Status,
            Timestamp = payment.CreatedAt
        });

        return payment.Id;
    }

    public async Task<PaymentResponse?> GetPaymentByIdAsync(Guid paymentId)
    {
        var payment = await _context.Payments.FindAsync(paymentId);
        if (payment is null) return null;

        return new PaymentResponse
        {
            Id = payment.Id,
            OrderId = payment.OrderId,
            Amount = payment.Amount,
            UserId = payment.UserId,
            Status = payment.Status,
            CreatedAt = payment.CreatedAt
        };
    }

    public async Task<bool> RefundAsync(Guid paymentId)
    {
        var payment = await _context.Payments.FindAsync(paymentId);
        if (payment is null) return false;

        payment.Status = "Refunded";
        await _context.SaveChangesAsync();

        await PublishKafkaEventAsync(new PaymentProcessedEvent
        {
            PaymentId = payment.Id,
            OrderId = payment.OrderId,
            Amount = payment.Amount,
            Status = payment.Status,
            Timestamp = DateTime.UtcNow
        });

        return true;
    }

    private async Task PublishKafkaEventAsync(PaymentProcessedEvent paymentEvent)
    {
        var bootstrapServers = _configuration["Kafka:BootstrapServers"];
        var topicName = _configuration["Kafka:TopicName"];

        var config = new ProducerConfig { BootstrapServers = bootstrapServers };

        using var producer = new ProducerBuilder<Null, string>(config).Build();

        var message = System.Text.Json.JsonSerializer.Serialize(paymentEvent);
        await producer.ProduceAsync(topicName, new Message<Null, string> { Value = message });

        _logger.LogInformation("Published payment event for PaymentId: {PaymentId}", paymentEvent.PaymentId);
    }
}
