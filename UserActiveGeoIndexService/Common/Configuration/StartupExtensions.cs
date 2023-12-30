using KafkaFlow;
using KafkaFlow.Serializer;
using UserActiveGeoIndexService.Events;
using UserActiveGeoIndexService.Handlers;
using AutoOffsetReset = KafkaFlow.AutoOffsetReset;

namespace UserActiveGeoIndexService.Common.Configuration;

public static class StartupExtensions
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
                            .Topic(KafkaConstants.UserLocationTopic)
                            .WithName($"{KafkaConstants.UserActiveGeoIndexTopic}-{Guid.NewGuid()}")
                            .WithGroupId(KafkaConstants.UserActiveGeoIndexTopic)
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