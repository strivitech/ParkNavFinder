using Kafka.Events.Contracts.Parking.Management;
using KafkaFlow;
using Microsoft.EntityFrameworkCore;
using Parking.StateService.Database;
using Parking.StateService.Domain;

namespace Parking.StateService.EventHandlers;

public class ParkingDeletedEventHandler : IMessageHandler<ParkingDeletedEvent>
{
    public async Task Handle(IMessageContext context, ParkingDeletedEvent deletedEvent)
    {
        var logger = context.DependencyResolver.Resolve<ILogger<ParkingDeletedEventHandler>>();
        var dbContext = context.DependencyResolver.Resolve<ParkingStateDbContext>();

        logger.LogDebug("ParkingDeletedEvent received: {ParkingId}", deletedEvent.ParkingId);

        var parkingState = await dbContext.ParkingStates.FirstOrDefaultAsync(ps => ps.ParkingId == deletedEvent.ParkingId.ToString());
        if (parkingState == null)
        {
            return;
        }

        dbContext.ParkingStates.Remove(parkingState);

        var geoIndexIsUsed = await dbContext.ParkingStates.AnyAsync(ps => ps.Index == parkingState.Index && ps.ParkingId != parkingState.ParkingId);
        if (!geoIndexIsUsed)
        {
            dbContext.GeoIndices.Remove(new GeoIndex { Index = parkingState.Index });
        }

        try
        {
            await dbContext.SaveChangesAsync();
            logger.LogDebug("ParkingDeletedEvent processed: {ParkingId}", deletedEvent.ParkingId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while processing ParkingDeletedEvent");
            throw;
        }
    }
}