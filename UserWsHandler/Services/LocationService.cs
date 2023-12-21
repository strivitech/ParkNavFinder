using UserWsHandler.Models;

namespace UserWsHandler.Services;

internal class LocationService : ILocationService
{
    public async Task SendLocation(Coordinate coordinate)
    {
        // TODO: Send location to Location Service using gRPC or REST
        await Task.CompletedTask;
    }
}