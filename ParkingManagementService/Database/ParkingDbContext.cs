using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace ParkingManagementService.Database;

public class ParkingDbContext(DbContextOptions<ParkingDbContext> options) : DbContext(options)
{
    public DbSet<Parking.Parking> ParkingSet => Set<Parking.Parking>();    
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}