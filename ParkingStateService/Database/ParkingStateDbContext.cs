using System.Reflection;
using Microsoft.EntityFrameworkCore;
using ParkingStateService.Parking;
using ParkingStateService.SpatialIndex;

namespace ParkingStateService.Database;

public class ParkingStateDbContext(DbContextOptions<ParkingStateDbContext> options) : DbContext(options)
{
    public DbSet<GeoIndexModel> ActiveIndices => Set<GeoIndexModel>();
    
    public DbSet<ParkingStateModel> ParkingStates => Set<ParkingStateModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}