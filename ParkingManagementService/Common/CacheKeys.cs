namespace ParkingManagementService.Common;

public static class CacheKeys
{
    public const string ParkingPrefix = "parking-";
    
    public static string ParkingKey(string parkingId) => $"{ParkingPrefix}{parkingId}";
}