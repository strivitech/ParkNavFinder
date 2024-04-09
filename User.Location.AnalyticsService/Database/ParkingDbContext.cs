using System.Reflection;
using Microsoft.EntityFrameworkCore;
using User.Location.AnalyticsService.Domain;

namespace User.Location.AnalyticsService.Database;

public class ParkingDbContext(DbContextOptions<ParkingDbContext> options) : DbContext(options)
{
    public DbSet<Parking> ParkingSet => Set<Parking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}