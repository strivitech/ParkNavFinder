namespace UserWsHandler.Models;

public record Parking(string Id, int FreeSpaces, int TotalSpaces, DateTime LastUpdated);