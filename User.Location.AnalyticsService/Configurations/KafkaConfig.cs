namespace User.Location.AnalyticsService.Configurations;

public class KafkaConfig
{
    public const string SectionName = "Kafka";
    
    public string Server { get; set; } = null!;
}