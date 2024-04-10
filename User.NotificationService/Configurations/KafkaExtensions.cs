using Kafka.Events.Contracts.Parking.State;
using Kafka.Settings;
using KafkaFlow;
using KafkaFlow.Serializer;
using User.NotificationService.EventHandlers;
using AutoOffsetReset = KafkaFlow.AutoOffsetReset;

namespace User.NotificationService.Configurations;

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
                            .Topic(TopicConfig.IndexStateEvents.TopicName)
                            .WithName($"{KafkaConstants.ConsumerName}-{Guid.NewGuid()}")
                            .WithGroupId(KafkaConstants.ConsumerName)
                            .WithAutoOffsetReset(AutoOffsetReset.Latest)
                            .WithBufferSize(1)
                            .WithWorkersCount(3)
                            .AddMiddlewares(middlewares => middlewares
                                .AddSingleTypeDeserializer<IndexStateChangedEvent, JsonCoreDeserializer>()
                                .AddTypedHandlers(handlers => handlers
                                    .WithHandlerLifetime(InstanceLifetime.Scoped)
                                    .AddHandler<IndexStateChangedEventHandler>()
                                )
                            )
                        )
                )
        );


        return services;
    }
}