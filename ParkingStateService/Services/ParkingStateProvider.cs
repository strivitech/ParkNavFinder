using Microsoft.EntityFrameworkCore;
using ParkingStateService.Database;
using ParkingStateService.Models;

namespace ParkingStateService.Services;

internal class ParkingStateProvider(IDbContextFactory<ParkingStateDbContext> dbContextFactory) : IParkingStateProvider
{
    private readonly IDbContextFactory<ParkingStateDbContext> _dbContextFactory = dbContextFactory;

    public async Task<List<ActiveParkingState>> GetParkingStatesAsync(string index)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.ParkingStates.Where(ps => ps.Index == index).ToListAsync();
    }
}