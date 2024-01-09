using Confluent.Kafka;
using Kafka.Settings;
using KafkaFlow;
using KafkaFlow.Serializer;
using SaslMechanism = KafkaFlow.Configuration.SaslMechanism;
using SecurityProtocol = KafkaFlow.Configuration.SecurityProtocol;

namespace LocationService.Kafka;

public static class KafkaExtensions
{
    public static IServiceCollection AddKafkaBroker(this IServiceCollection services, IConfiguration configuration,
        IHostEnvironment environment)
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
                            .CreateTopicIfNotExists(TopicConfig.UserLocations.TopicName, TopicConfig.UserLocations.NumberOfPartitions,
                                TopicConfig.UserLocations.ReplicationFactor)
                            .AddProducer(
                                KafkaConstants.ProducerName,
                                producer => producer
                                    .DefaultTopic(TopicConfig.UserLocations.TopicName)
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
                                    .DefaultTopic(TopicConfig.UserLocations.TopicName)
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