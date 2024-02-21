using Auth.Shared;
using DataManager.Api.Contracts;
using DataManager.Api.Services;

namespace DataManager.Api.Common;

public static class ParkingGeneratorExtensions
{
    public static WebApplication EnsureParkingCreated(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var parkingManager = scope.ServiceProvider.GetRequiredService<IParkingManager>();
        var userManager = scope.ServiceProvider.GetRequiredService<IUserManager>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var providers = userManager.GetUsersAsync(new GetUsersRequest(Roles.Provider)).Result
            .Where(x => x.Email.Contains(Constants.GeneratedEmailSharedKey)).ToList();
        var tokenStorage = scope.ServiceProvider.GetRequiredService<ITokenStorage>();
        var parkingGenerator = scope.ServiceProvider.GetRequiredService<IParkingGenerator>();
        var parkingCount = configuration.GetValue<int>("Generator:ParkingCountForEachProvider");

        foreach (var token in providers.Select(provider => tokenStorage.GetToken(provider.UserId)))
        {
            CreateParkingForProvider(parkingManager, parkingGenerator, token, parkingCount);
        }

        return app;
    }

    private static void CreateParkingForProvider(IParkingManager parkingManager, IParkingGenerator parkingGenerator,
        string token, int count)
    {
        foreach (var parking in parkingGenerator.GenerateRealisticParkingData(count))
        {
            parkingManager.CreateAsync(parking, token).Wait();
        }
    }
}