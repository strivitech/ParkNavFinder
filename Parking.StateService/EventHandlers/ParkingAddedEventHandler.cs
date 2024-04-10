using Kafka.Events.Contracts.Parking.Management;
using KafkaFlow;
using Microsoft.EntityFrameworkCore;
using Parking.StateService.Database;
using Parking.StateService.Domain;

namespace Parking.StateService.EventHandlers;

public class ParkingAddedEventHandler : IMessageHandler<ParkingAddedEvent>
{
    public async Task Handle(IMessageContext context, ParkingAddedEvent addedEvent)
    {
        var logger = context.DependencyResolver.Resolve<ILogger<ParkingAddedEventHandler>>();
        var dbContext = context.DependencyResolver.Resolve<ParkingStateDbContext>();

        logger.LogDebug("ParkingAddedEvent received: {ParkingId}", addedEvent.ParkingId);

        if (!await GeoIndexExistsAsync(dbContext, addedEvent.GeoIndex))
        {
            AddGeoIndex(dbContext, addedEvent.GeoIndex);
        }

        AddParkingState(dbContext, addedEvent, addedEvent.GeoIndex);

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
            GeoIndex = newGeoIndex,
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