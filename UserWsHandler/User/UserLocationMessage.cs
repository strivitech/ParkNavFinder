namespace UserWsHandler.User;

public record UserLocationMessage(string UserId, double Latitude, double Longitude, DateTime Timestamp);