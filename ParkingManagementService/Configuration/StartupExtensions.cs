using Confluent.Kafka;
using KafkaFlow;
using KafkaFlow.Serializer;

namespace ParkingManagementService.Configuration;

public static class StartupExtensions
{
    public static IServiceCollection AddKafkaBroker(this IServiceCollection services, IConfiguration configuration)
    {
        var kafkaConfig = configuration.GetRequiredSection(KafkaConfig.SectionName).Get<KafkaConfig>()!;
    
        services.AddKafka(
            kafka => kafka
                .UseMicrosoftLog()
                .AddCluster(
                    cluster => cluster
                        .WithBrokers(new[] { kafkaConfig.Server })
                        .CreateTopicIfNotExists(KafkaConstants.ParkingManagementEvents, 1, 1)
                        .AddProducer(
                            KafkaConstants.ProducerName,
                            producer => producer
                                .DefaultTopic(KafkaConstants.ParkingManagementEvents)
                                .AddMiddlewares(m =>
                                    m.AddSerializer<JsonCoreSerializer>()
                                )
                                .WithCompression(CompressionType.Gzip)
                        )
                )
        );


        return services;
    }
}