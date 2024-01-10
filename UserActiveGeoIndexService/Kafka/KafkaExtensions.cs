using Kafka.Events.Contracts.User.Location;
using Kafka.Settings;
using KafkaFlow;
using KafkaFlow.Serializer;
using UserActiveGeoIndexService.UserLocation;
using AutoOffsetReset = KafkaFlow.AutoOffsetReset;

namespace UserActiveGeoIndexService.Kafka;

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