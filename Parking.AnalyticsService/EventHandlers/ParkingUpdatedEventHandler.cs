using Kafka.Events.Contracts.Parking.Management;
using KafkaFlow;
using Microsoft.EntityFrameworkCore;
using Parking.AnalyticsService.Database;

namespace Parking.AnalyticsService.EventHandlers;

public class ParkingUpdatedEventHandler : IMessageHandler<ParkingUpdatedEvent>
{
    public async Task Handle(IMessageContext context, ParkingUpdatedEvent updatedEvent)
    {
        var logger = context.DependencyResolver.Resolve<ILogger<ParkingUpdatedEventHandler>>();
        var dbContext = context.DependencyResolver.Resolve<ParkingDbContext>();

        logger.LogDebug("ParkingUpdatedEvent received: {ParkingId}", updatedEvent.ParkingId);

        var parkingState = await dbContext.ParkingSet.FirstOrDefaultAsync(ps => ps.ParkingId == updatedEvent.ParkingId.ToString());
        if (parkingState is null)
        {
            logger.LogError("Parking state not found for ParkingId: {ParkingId}", updatedEvent.ParkingId);
            return;
        }

        parkingState.TotalSpaces = updatedEvent.TotalSpaces;

        try
        {
            await dbContext.SaveChangesAsync();
            logger.LogDebug("ParkingUpdatedEvent processed: {ParkingId}", updatedEvent.ParkingId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while processing ParkingUpdatedEvent");
            throw;
        }
    }
}