namespace User.NotificationService.Contracts;

public record UserHandlerDescription
{
    public string UserId { get; init; } = null!; 
    public string? Handler { get; init; }
}