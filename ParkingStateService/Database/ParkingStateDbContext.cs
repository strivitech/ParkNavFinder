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
        
        modelBuilder.Entity<ActiveIndex>().HasData(SeedActiveIndices());

        base.OnModelCreating(modelBuilder);
    }
    
    private static ActiveIndex[] SeedActiveIndices()
    {
        var activeIndices = new List<ActiveIndex>();
        for (var i = 1; i <= 250; i++)
        {
            activeIndices.Add(new ActiveIndex { Index = $"H3Index_{i}", LastUpdatedUtc = DateTime.UtcNow.AddMinutes(-7) });
        }

        return activeIndices.ToArray();
    }
}