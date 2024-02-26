using Auth.Shared;
using DataManager.Api.Services;

namespace DataManager.Api.Common;

public static class UserGeneratorExtensions
{
    public static WebApplication EnsureUsersCreated(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var userGenerator = scope.ServiceProvider.GetRequiredService<IUserGenerator>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var userManager = scope.ServiceProvider.GetRequiredService<IUserManager>();
        int userCount = configuration.GetValue<int>("Generator:UserCount");
        int providerCount = configuration.GetValue<int>("Generator:ProviderCount");
        int adminCount = configuration.GetValue<int>("Generator:AdminCount");

        var currentUserCount = userManager.CountGeneratedUsersAsync(Roles.User).Result;
        var currentProviderCount = userManager.CountGeneratedUsersAsync(Roles.Provider).Result;
        var currentAdminCount = userManager.CountGeneratedUsersAsync(Roles.Admin).Result;

        GenerateUsers(userGenerator, Roles.User, userCount, currentUserCount);
        GenerateUsers(userGenerator, Roles.Provider, providerCount, currentProviderCount);
        GenerateUsers(userGenerator, Roles.Admin, adminCount, currentAdminCount);
        
        return app;
    }

    private static void GenerateUsers(IUserGenerator generator, string role, int count, int currentCount)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(count, 0);

        if (count > 0 && currentCount < count)
        {
            generator.GenerateAsync(role, count - currentCount).Wait();
        }
    }
}