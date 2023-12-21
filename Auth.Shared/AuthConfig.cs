namespace Auth.Shared;

public record AuthConfig
{
    public required string Authority { get; set; }
    public required string Audience { get; set; }
}