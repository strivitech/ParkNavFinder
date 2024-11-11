namespace Parking.AnalyticsService.Common;

public static class Constants
{
    public const int UpdateDelaySeconds = 45; // Must be slightly less than the ParkingStateChangerServiceCronExpression

    public const string ParkingStateChangerServiceCronExpression = "*/1 * * * *"; // Every minute
    
    public const int MaxParkingBatchSize = 100;
}