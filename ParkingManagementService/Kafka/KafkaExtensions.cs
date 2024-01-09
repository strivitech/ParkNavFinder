using Confluent.Kafka;
using Kafka.Settings;
using KafkaFlow;
using KafkaFlow.Serializer;

namespace ParkingManagementService.Kafka;

public static class KafkaExtensions
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
                        .CreateTopicIfNotExists(TopicConfig.ParkingManagementEvents.TopicName,
                            TopicConfig.ParkingManagementEvents.NumberOfPartitions,
                            TopicConfig.ParkingManagementEvents.ReplicationFactor)
                        .AddProducer(
                            KafkaConstants.ProducerName,
                            producer => producer
                                .DefaultTopic(TopicConfig.ParkingManagementEvents.TopicName)
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