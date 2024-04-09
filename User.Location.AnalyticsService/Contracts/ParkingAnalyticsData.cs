namespace User.Location.AnalyticsService.Contracts;
    
public record ParkingAnalyticsData(string ParkingId, int TotalObservers, double Probability, DateTime CalculatedAtUtc); 