using Kafka.Events.Contracts.Parking.Management;
using KafkaFlow;
using Microsoft.EntityFrameworkCore;
using User.Location.AnalyticsService.Database;

namespace User.Location.AnalyticsService.EventHandlers;

public class ParkingDeletedEventHandler : IMessageHandler<ParkingDeletedEvent>
{
    public async Task Handle(IMessageContext context, ParkingDeletedEvent deletedEvent)
    {
        var logger = context.DependencyResolver.Resolve<ILogger<ParkingDeletedEventHandler>>();
        var dbContext = context.DependencyResolver.Resolve<ParkingDbContext>();

        logger.LogDebug("ParkingDeletedEvent received: {ParkingId}", deletedEvent.ParkingId);

        var parkingState = await dbContext.ParkingSet.FirstOrDefaultAsync(ps => ps.ParkingId == deletedEvent.ParkingId.ToString());
        if (parkingState == null)
        {
            return;
        }

        dbContext.ParkingSet.Remove(parkingState);

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