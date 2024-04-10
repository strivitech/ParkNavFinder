using Confluent.Kafka;
using Kafka.Settings;
using KafkaFlow;
using KafkaFlow.Serializer;
using Parking.StateService.EventHandlers;
using AutoOffsetReset = KafkaFlow.AutoOffsetReset;

namespace Parking.StateService.Configurations;

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
                        .CreateTopicIfNotExists(TopicConfig.IndexStateEvents.TopicName, TopicConfig.IndexStateEvents.NumberOfPartitions,
                            TopicConfig.IndexStateEvents.ReplicationFactor)
                        .AddProducer(
                            KafkaConstants.ProducerName,
                            producer => producer
                                .DefaultTopic(TopicConfig.IndexStateEvents.TopicName)
                                .AddMiddlewares(m =>
                                    m.AddSerializer<JsonCoreSerializer>()
                                )
                                .WithCompression(CompressionType.Gzip)
                        )
                        .AddConsumer(consumer => consumer
                            .Topic(TopicConfig.ParkingManagementEvents.TopicName)
                            .WithName($"{KafkaConstants.ConsumerName}-{Guid.NewGuid()}")
                            .WithGroupId(KafkaConstants.ConsumerName)
                            .WithAutoOffsetReset(AutoOffsetReset.Latest)
                            .WithBufferSize(1)
                            .WithWorkersCount(1)
                            .AddMiddlewares(middlewares => middlewares
                                .AddDeserializer<JsonCoreDeserializer>()
                                .AddTypedHandlers(handlers => handlers
                                        .WithHandlerLifetime(InstanceLifetime.Scoped)
                                        .AddHandler<ParkingAddedEventHandler>()
                                        .AddHandler<ParkingDeletedEventHandler>()
                                )
                            )
                        )
                )
        );


        return services;
    }
}