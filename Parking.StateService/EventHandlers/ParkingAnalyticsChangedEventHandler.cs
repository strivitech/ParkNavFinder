using EFCore.BulkExtensions;
using Kafka.Events.Contracts.Parking.State;
using KafkaFlow;
using Microsoft.EntityFrameworkCore;
using Parking.StateService.Database;
using Parking.StateService.Domain;

namespace Parking.StateService.EventHandlers;

public class ParkingAnalyticsChangedEventHandler : IMessageHandler<ParkingAnalyticsChangedEvent>
{
    public async Task Handle(IMessageContext context, ParkingAnalyticsChangedEvent message)
    {
        var logger = context.DependencyResolver.Resolve<ILogger<ParkingAnalyticsChangedEventHandler>>();
        var dbContextFactory = context.DependencyResolver.Resolve<IDbContextFactory<ParkingStateDbContext>>();
        logger.LogDebug("ParkingAnalyticsChangedEvent received");

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        var parkingStatesToUpdate = message.ParkingStateInfos
            .Select(psi => new ParkingState
            {
                ParkingId = psi.ParkingId,
                TotalObservers = psi.TotalObservers,
                Probability = psi.Probability,
                LastCalculatedUtc = psi.LastCalculatedUtc
            }).ToList();

        await dbContext.BulkUpdateAsync(parkingStatesToUpdate,
            new BulkConfig { PropertiesToExclude = new List<string> { nameof(ParkingState.GeoIndex) } });

        logger.LogDebug("Parking states updated");
    }
}