using Confluent.Kafka;
using KafkaFlow;
using KafkaFlow.Serializer;
using SaslMechanism = KafkaFlow.Configuration.SaslMechanism;
using SecurityProtocol = KafkaFlow.Configuration.SecurityProtocol;

namespace LocationService.Common.Configuration;

public static class StartupExtensions
{
    public static IServiceCollection AddKafkaBroker(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        var kafkaConfig = configuration.GetRequiredSection(KafkaConfig.SectionName).Get<KafkaConfig>()!;

        if (environment.IsDevelopment())
        {
            services.AddKafka(
                kafka => kafka
                    .UseMicrosoftLog()
                    .AddCluster(
                        cluster => cluster
                            .WithBrokers(new[] { kafkaConfig.Server })
                            .CreateTopicIfNotExists(KafkaConstants.UserLocationTopic, 1, 1)
                            .AddProducer(
                                KafkaConstants.ProducerName,
                                producer => producer
                                    .DefaultTopic(KafkaConstants.UserLocationTopic)
                                    .AddMiddlewares(m =>
                                        m.AddSerializer<JsonCoreSerializer>()
                                    )
                                    .WithCompression(CompressionType.Gzip)
                            )
                    )
            );
        }
        else
        {
            // Manually create the topic in the real Confluent Kafka cluster

            services.AddKafka(
                kafka => kafka
                    .UseMicrosoftLog()
                    .AddCluster(
                        cluster => cluster
                            .WithBrokers(new[] { kafkaConfig.Server })
                            .WithSecurityInformation(si =>
                            {
                                si.SecurityProtocol = (SecurityProtocol?)Confluent.Kafka.SecurityProtocol.SaslSsl;
                                si.SaslMechanism = (SaslMechanism?)Confluent.Kafka.SaslMechanism.Plain;
                                si.SaslUsername = kafkaConfig.Username;
                                si.SaslPassword = kafkaConfig.Password;
                            })
                            .AddProducer(
                                KafkaConstants.ProducerName,
                                producer => producer
                                    .DefaultTopic(KafkaConstants.UserLocationTopic)
                                    .AddMiddlewares(m =>
                                        m.AddSerializer<JsonCoreSerializer>()
                                    )
                                    .WithCompression(CompressionType.Gzip)
                            )
                    )
            );
        }

        return services;
    }
}