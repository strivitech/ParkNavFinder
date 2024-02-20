using System.Net.Http.Headers;
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
        var parkingCount = configuration.GetValue<int>("Generator:ParkingCountForEachProvider");
        
        foreach (var token in providers.Select(provider => tokenStorage.GetToken(provider.UserId)))
        {
            CreateParkingForProvider(parkingManager, token, parkingCount);
        }
        
        return app;
    }

    private static void CreateParkingForProvider(IParkingManager parkingManager, string token, int count)
    {
        foreach (var parking in GenerateRandomParkingData(count))
        {
            parkingManager.CreateAsync(parking, token).Wait();
        }
    }
    
    private static IEnumerable<CreateParkingRequest> GenerateRandomParkingData(int count)
    {
        for (int i = 0; i < count; i++)
        {
            yield return new CreateParkingRequest(
                Name: RandomString(1, 100),
                Description: RandomString(1, 2000),
                Address: new Address(
                    Country: RandomString(1, 100),
                    City: RandomString(1, 100),
                    Street: RandomString(1, 200),
                    StreetNumber: RandomString(1, 10)),
                Latitude: RandomDouble(-90, 90),
                Longitude: RandomDouble(-180, 180),
                TotalSpaces: Random.Shared.Next(1, 10000));
        }
    }

    private static string RandomString(int minLength, int maxLength)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        int length = Random.Shared.Next(minLength, maxLength + 1);
        char[] stringChars = new char[length];
        for (int i = 0; i < length; i++)
        {
            stringChars[i] = chars[Random.Shared.Next(chars.Length)];
        }
        return new string(stringChars);
    }

    private static double RandomDouble(double min, double max)
    {
        return min + Random.Shared.NextDouble() * (max - min);
    }
}