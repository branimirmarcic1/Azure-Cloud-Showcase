using Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace OrderAuditorFunction;

public class OrderCreatedConsumer : IConsumer<OrderCreated>
{
    private readonly ILogger<OrderCreatedConsumer> _logger;
    private readonly IMongoClient _mongoClient;

    public OrderCreatedConsumer(ILogger<OrderCreatedConsumer> logger, IMongoClient mongoClient)
    {
        _logger = logger;
        _mongoClient = mongoClient;
    }

    public async Task Consume(ConsumeContext<OrderCreated> context)
    {
        _logger.LogInformation("--> Primljen OrderCreated događaj: {OrderId}", context.Message.OrderId);

        var db = _mongoClient.GetDatabase("AuditDb");
        var collection = db.GetCollection<OrderCreated>("OrderAudits");

        await collection.InsertOneAsync(context.Message);

        _logger.LogInformation("--> Zapis o narudžbi {OrderId} spremljen u AuditDb.", context.Message.OrderId);
    }
}
