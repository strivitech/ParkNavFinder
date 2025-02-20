﻿using System.Data;
using Microsoft.EntityFrameworkCore;
using Parking.AnalyticsService.Common;
using Parking.AnalyticsService.Database;

namespace Parking.AnalyticsService.Services;

public class ParkingRetrieverService(
    ParkingDbContext dbContext,
    ILogger<ParkingRetrieverService> logger) : IParkingRetrieverService
{
    private readonly ParkingDbContext _dbContext = dbContext;
    private readonly ILogger<ParkingRetrieverService> _logger = logger;

    public async Task<List<Domain.Parking>> GetNextParkingList()
    {
        var strategy = _dbContext.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction =
                    await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);
                try
                {
                    _logger.LogDebug("Retrieving parkings");
                     var parkingList = await _dbContext.Database.SqlQuery<Domain.Parking>
                          (  
                              $"""
                               SELECT * FROM "ParkingSet"
                               WHERE "LastCalculatedUtc" < {DateTime.UtcNow.AddSeconds(-Constants.UpdateDelaySeconds)}
                               ORDER BY "GeoIndex" ASC, "LastCalculatedUtc" ASC
                               FOR UPDATE SKIP LOCKED
                               LIMIT {Constants.MaxParkingBatchSize}
                               """)
                          .ToListAsync();

                    var parkingIdList = parkingList.Select(x => x.GeoIndex).ToList();
                    
                    var dateTimeUtc = DateTime.UtcNow;
                    await _dbContext.ParkingSet
                        .Where(x => parkingIdList.Contains(x.GeoIndex))
                        .ExecuteUpdateAsync(setters =>
                            setters.SetProperty(x => x.LastCalculatedUtc, dateTimeUtc));
                    
                    await transaction.CommitAsync();
                    _logger.LogDebug("Retrieved {Count} parkings", parkingList.Count);
                    return parkingList;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to retrieve parking indices");
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        );
    }
}