namespace UserActiveGeoIndexService.GeoIndex;

public interface IGeoIndexService
{
    Task<string> GetGeoIndexAsync(double lat, double lon, int resolution);  
}