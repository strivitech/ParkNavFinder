using Microsoft.EntityFrameworkCore;
using Parking.StateService.Database;
using Parking.StateService.Domain;

namespace Parking.StateService.Services;

public class ParkingStateProvider(IDbContextFactory<ParkingStateDbContext> dbContextFactory) : IParkingStateProvider
{
    private readonly IDbContextFactory<ParkingStateDbContext> _dbContextFactory = dbContextFactory;

    public async Task<List<ParkingState>> GetParkingStatesAsync(string index)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.ParkingStates.Where(ps => ps.Index == index).ToListAsync();
    }
}