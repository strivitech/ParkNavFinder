using System.Reflection;
using Microsoft.EntityFrameworkCore;
using ParkingManagementService.Parking;

namespace ParkingManagementService.Database;

public class ParkingDbContext(DbContextOptions<ParkingDbContext> options) : DbContext(options)
{
    public DbSet<ParkingModel> ParkingSet => Set<ParkingModel>();    
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}