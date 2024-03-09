namespace Parking.ManagementService.Services;

public interface IMapService
{
    Task<string> GetIndexAsync(double lat, double lon, int resolution);  
}