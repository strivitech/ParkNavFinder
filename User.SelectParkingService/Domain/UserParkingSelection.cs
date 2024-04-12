namespace User.SelectParkingService.Domain;

public class UserParkingSelection
{
    public Guid Id { get; set; }
    
    public string UserId { get; set; } = null!;

    public Guid ParkingId { get; set; }
    
    public DateTime DateTimeUtc { get; set; }   
}