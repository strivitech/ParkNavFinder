namespace UserActiveGeoIndexService.Kafka;

public class KafkaConfig
{
    public const string SectionName = "Kafka";
    
    public string Server { get; set; } = null!;
}