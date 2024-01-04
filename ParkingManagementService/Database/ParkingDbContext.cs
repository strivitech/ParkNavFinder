using System.Reflection;
using Microsoft.EntityFrameworkCore;
using ParkingManagementService.Models;

namespace ParkingManagementService.Database;

public class ParkingDbContext(DbContextOptions<ParkingDbContext> options) : DbContext(options)
{
    public DbSet<Parking> ParkingSet => Set<Parking>();    
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}