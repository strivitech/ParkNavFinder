using System.Net.Http.Headers;
using Auth.Shared;
using Bogus;
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
        var parkingCount = configuration.GetValue<int>("Generator:ParkingCountForEachProvider");
        
        foreach (var token in providers.Select(provider => tokenStorage.GetToken(provider.UserId)))
        {
            CreateParkingForProvider(parkingManager, token, parkingCount);
        }
        
        return app;
    }

    private static void CreateParkingForProvider(IParkingManager parkingManager, string token, int count)
    {
        foreach (var parking in GenerateRealisticParkingData(count))
        {
            parkingManager.CreateAsync(parking, token).Wait();
        }
    }
    
    private static IEnumerable<CreateParkingRequest> GenerateRealisticParkingData(int count)
    {
        var faker = new Faker();
        for (int i = 0; i < count; i++)
        {
            var address = faker.Address;
            yield return new CreateParkingRequest(
                Name: faker.Company.CompanyName(),
                Description: faker.Lorem.Paragraph(),
                Address: new Address(
                    Country: address.Country(),
                    City: address.City(),
                    Street: address.StreetName(),
                    StreetNumber: address.BuildingNumber()),
                Latitude: address.Latitude(50.30, 50.52),
                Longitude: address.Longitude(30.35, 30.67),
                TotalSpaces: faker.Random.Int(10, 500));
        }
    }
}