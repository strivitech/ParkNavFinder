using Kafka.Events.Contracts.Parking.Management;
using KafkaFlow;
using Microsoft.EntityFrameworkCore;
using Parking.StateService.Common;
using Parking.StateService.Database;
using Parking.StateService.Domain;
using Parking.StateService.Services;

namespace Parking.StateService.EventHandlers;

public class ParkingAddedEventHandler : IMessageHandler<ParkingAddedEvent>
{
    public async Task Handle(IMessageContext context, ParkingAddedEvent addedEvent)
    {
        var logger = context.DependencyResolver.Resolve<ILogger<ParkingAddedEventHandler>>();
        var geoIndexService = context.DependencyResolver.Resolve<IGeoIndexService>();
        var dbContext = context.DependencyResolver.Resolve<ParkingStateDbContext>();

        logger.LogDebug("ParkingAddedEvent received: {ParkingId}", addedEvent.ParkingId);

        var newGeoIndex = await geoIndexService.GetGeoIndexAsync(addedEvent.Latitude, addedEvent.Longitude,
            Constants.GeoIndexResolution);

        if (!await GeoIndexExistsAsync(dbContext, newGeoIndex))
        {
            AddGeoIndex(dbContext, newGeoIndex);
        }

        AddParkingState(dbContext, addedEvent, newGeoIndex);

        try
        {
            await dbContext.SaveChangesAsync();
            logger.LogDebug("ParkingAddedEvent processed: {ParkingId}", addedEvent.ParkingId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while saving changes to the database");
            throw;
        }
    }

    private static async Task<bool> GeoIndexExistsAsync(ParkingStateDbContext dbContext, string geoIndex) =>
        await dbContext.GeoIndices.AnyAsync(gi => gi.Index == geoIndex);

    private static void AddParkingState(ParkingStateDbContext dbContext, ParkingAddedEvent addedEvent,
        string newGeoIndex)
    {
        dbContext.ParkingStates.Add(new ParkingState
        {
            ParkingId = addedEvent.ParkingId.ToString(),
            Index = newGeoIndex,
            LastCalculatedUtc = default
        });
    }

    private static void AddGeoIndex(ParkingStateDbContext dbContext, string newGeoIndex)
    {
        dbContext.GeoIndices.Add(new GeoIndex
        {
            Index = newGeoIndex,
            LastUpdatedUtc = default
        });
    }
}