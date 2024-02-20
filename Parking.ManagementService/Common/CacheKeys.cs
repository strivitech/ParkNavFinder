namespace Parking.ManagementService.Common;

public static class CacheKeys
{
    public const string ParkingPrefix = "parking-";
    
    public static string ParkingKey(string parkingId) => $"{ParkingPrefix}{parkingId}";
    
    public static string ProviderParkingKey(string providerId) => $"{ParkingPrefix}-provider-{providerId}";
}