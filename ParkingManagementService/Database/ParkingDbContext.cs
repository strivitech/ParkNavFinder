using Microsoft.EntityFrameworkCore;
using ParkingManagementService.Models;

namespace ParkingManagementService.Database;

public class ParkingDbContext(DbContextOptions<ParkingDbContext> options) : DbContext(options)
{
    public DbSet<Parking> ParkingSet => Set<Parking>();         
}