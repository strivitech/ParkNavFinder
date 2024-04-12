using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace User.SelectParkingService.Database;

public class ParkingSelectionDbContext(DbContextOptions<ParkingSelectionDbContext> options) : DbContext(options)
{
    public DbSet<Domain.UserParkingSelection> UserParkingSelections => Set<Domain.UserParkingSelection>();    
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}