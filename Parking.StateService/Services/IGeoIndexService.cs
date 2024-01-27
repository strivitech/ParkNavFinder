namespace Parking.StateService.Services;

public interface IGeoIndexService
{
    Task<string> GetGeoIndexAsync(double lat, double lon, int resolution);  
}