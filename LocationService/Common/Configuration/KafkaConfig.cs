namespace LocationService.Common.Configuration;

public class KafkaConfig
{
    public const string SectionName = "Kafka";
    
    public string Server { get; set; } = null!;
    
    public string Username { get; set; } = null!;
    
    public string Password { get; set; } = null!;
}