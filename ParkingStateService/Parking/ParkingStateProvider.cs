using Microsoft.EntityFrameworkCore;
using ParkingStateService.Database;

namespace ParkingStateService.Parking;

internal class ParkingStateProvider(IDbContextFactory<ParkingStateDbContext> dbContextFactory) : IParkingStateProvider
{
    private readonly IDbContextFactory<ParkingStateDbContext> _dbContextFactory = dbContextFactory;

    public async Task<List<ParkingStateModel>> GetParkingStatesAsync(string index)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.ParkingStates.Where(ps => ps.Index == index).ToListAsync();
    }
}