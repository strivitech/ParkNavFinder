namespace Parking.AnalyticsService.Services;

public interface IParkingAnalyticsArchivalService
{
    Task ArchiveAsync<T>(T data);
}