namespace Parking.StateService.Common;

public static class Constants
{
    public const double UpdateDelaySeconds = 45; // Must be slightly less than the GeoIndexStateNotificationJobCronExpression
    
    public const string GeoIndexStateNotificationJobCronExpression = "*/1 * * * *"; // Every minute
    
    public const int MaxParkingIndicesPerUpdate = 100;
}