namespace UserWsHandler.Models;

public record UserLocationMessage(string UserId, double Latitude, double Longitude, DateTime Timestamp);