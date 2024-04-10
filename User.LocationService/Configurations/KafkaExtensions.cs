using Confluent.Kafka;
using Kafka.Events.Contracts.User.Location;
using Kafka.Settings;
using KafkaFlow;
using KafkaFlow.Serializer;
using User.LocationService.EventHandlers;
using AutoOffsetReset = KafkaFlow.AutoOffsetReset;

namespace User.LocationService.Configurations;

public static class KafkaExtensions
{
    public static IServiceCollection AddKafkaBroker(this IServiceCollection services, IConfiguration configuration)
    {
        var kafkaConfig = configuration.GetRequiredSection(KafkaConfig.SectionName).Get<KafkaConfig>()!;

        services.AddKafkaFlowHostedService(
            kafka => kafka
                .UseMicrosoftLog()
                .AddCluster(
                    cluster => cluster
                        .WithBrokers(new[] { kafkaConfig.Server })
                        .CreateTopicIfNotExists(TopicConfig.UserLocationAreasAnalytics.TopicName, TopicConfig.UserLocationAreasAnalytics.NumberOfPartitions,
                            TopicConfig.UserLocationAreasAnalytics.ReplicationFactor)
                        .AddProducer(
                            KafkaConstants.ProducerName,
                            producer => producer
                                .DefaultTopic(TopicConfig.UserLocationAreasAnalytics.TopicName)
                                .AddMiddlewares(m =>
                                    m.AddSerializer<JsonCoreSerializer>()
                                )
                                .WithCompression(CompressionType.Gzip)
                        )
                        .AddConsumer(consumer => consumer
                            .Topic(TopicConfig.UserLocations.TopicName)
                            .WithName($"{KafkaConstants.ConsumerName}-{Guid.NewGuid()}")
                            .WithGroupId(KafkaConstants.ConsumerName)
                            .WithBufferSize(100)
                            .WithWorkersCount(3)
                            .WithAutoOffsetReset(AutoOffsetReset.Latest)
                            .AddMiddlewares(middlewares => middlewares
                                .AddSingleTypeDeserializer<UserLocationChangedEvent, JsonCoreDeserializer>()
                                .AddTypedHandlers(handlers => handlers
                                    .AddHandler<UserLocationChangedEventHandler>()
                                )
                            )
                        )
                ));

        return services;
    }
}