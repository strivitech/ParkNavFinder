namespace ParkingStateService.Database;

public static class MigrationExtension
{
    public static void EnsureDbCreated(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ParkingStateDbContext>();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }
}