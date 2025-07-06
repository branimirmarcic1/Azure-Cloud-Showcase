using MassTransit;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using OrderAuditorFunction;

// Konfiguracija za spremanje Guid-a kao stringa u MongoDB
BsonSerializer.RegisterSerializer(new GuidSerializer(MongoDB.Bson.BsonType.String));

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        // Registracija MassTransit-a
        services.AddMassTransit(x =>
        {
            x.AddConsumer<OrderCreatedConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.ReceiveEndpoint("order-audit-queue", e =>
                {
                    e.ConfigureConsumer<OrderCreatedConsumer>(context);
                });
            });
        });

        // Registracija MongoDB klijenta
        services.AddSingleton<IMongoClient>(sp =>
            new MongoClient(hostContext.Configuration.GetConnectionString("MongoDb")));
    })
    .Build();

host.Run();
