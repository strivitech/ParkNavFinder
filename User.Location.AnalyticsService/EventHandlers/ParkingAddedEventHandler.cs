using Kafka.Events.Contracts.Parking.Management;
using KafkaFlow;
using User.Location.AnalyticsService.Database;
using User.Location.AnalyticsService.Domain;

namespace User.Location.AnalyticsService.EventHandlers;

public class ParkingAddedEventHandler : IMessageHandler<ParkingAddedEvent>
{
    public async Task Handle(IMessageContext context, ParkingAddedEvent addedEvent)
    {
        var logger = context.DependencyResolver.Resolve<ILogger<ParkingAddedEventHandler>>();
        var dbContext = context.DependencyResolver.Resolve<ParkingDbContext>();

        logger.LogDebug("ParkingAddedEvent received: {ParkingId}", addedEvent.ParkingId);

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

    private static void AddParkingState(ParkingDbContext dbContext, ParkingAddedEvent addedEvent,
        string newGeoIndex)
    {
        dbContext.ParkingSet.Add(new Parking
        {
            ParkingId = addedEvent.ParkingId.ToString(),
            GeoIndex = newGeoIndex,
            TotalSpaces = addedEvent.TotalSpaces,
            Latitude = addedEvent.Latitude,
            Longitude = addedEvent.Longitude
        });
    }
}