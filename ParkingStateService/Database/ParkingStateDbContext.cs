using System.Reflection;
using Microsoft.EntityFrameworkCore;
using ParkingStateService.Models;

namespace ParkingStateService.Database;

public class ParkingStateDbContext(DbContextOptions<ParkingStateDbContext> options) : DbContext(options)
{
    public DbSet<ActiveIndex> ActiveIndices => Set<ActiveIndex>();
    
    public DbSet<ActiveParkingState> ParkingStates => Set<ActiveParkingState>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}