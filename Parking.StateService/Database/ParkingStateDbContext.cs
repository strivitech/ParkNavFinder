using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Parking.StateService.Domain;

namespace Parking.StateService.Database;

public class ParkingStateDbContext(DbContextOptions<ParkingStateDbContext> options) : DbContext(options)
{
    public DbSet<GeoIndex> GeoIndices => Set<GeoIndex>();
    
    public DbSet<ParkingState> ParkingStates => Set<ParkingState>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}