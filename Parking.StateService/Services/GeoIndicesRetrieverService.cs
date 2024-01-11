using System.Data;
using Microsoft.EntityFrameworkCore;
using Parking.StateService.Common;
using Parking.StateService.Database;

namespace Parking.StateService.Services;

internal class GeoIndicesRetrieverService(
    ParkingStateDbContext dbContext,
    ILogger<GeoIndicesRetrieverService> logger) : IGeoIndicesRetrieverService
{
    private readonly ParkingStateDbContext _dbContext = dbContext;
    private readonly ILogger<GeoIndicesRetrieverService> _logger = logger;

    public async Task<IList<string>> GetNextParkingIndices()
    {
        var strategy = _dbContext.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction =
                    await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);
                try
                {
                    _logger.LogDebug("Retrieving parking indices");
                     var indices = await _dbContext.Database.SqlQuery<string>
                         (
                             $"""
                              SELECT "Index" FROM "GeoIndices"
                              WHERE "LastUpdatedUtc" < {DateTime.UtcNow.AddMinutes(-Constants.UpdateIntervalMinutes)}
                              ORDER BY "LastUpdatedUtc" ASC
                              FOR UPDATE SKIP LOCKED
                              LIMIT {Constants.MaxParkingIndicesPerUpdate}
                              """)
                         .ToListAsync();

                    var dateTimeUtc = DateTime.UtcNow;
                    await _dbContext.GeoIndices
                        .Where(x => indices.Contains(x.Index))
                        .ExecuteUpdateAsync(setters =>
                            setters.SetProperty(x => x.LastUpdatedUtc, dateTimeUtc));

                    await transaction.CommitAsync();
                    _logger.LogDebug("Retrieved {Count} parking indices", indices.Count);
                    return indices;
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