using Auth.Shared;
using DataManager.Api.Contracts;
using DataManager.Api.Services;

namespace DataManager.Api.Common;

public static class AppExtensions
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

        var currentUserCount = GetCurrentUserCount(userManager);
        var currentProviderCount = GetCurrentProviderCount(userManager);
        var currentAdminCount = GetCurrentAdminCount(userManager);

        GenerateUsers(userGenerator, Roles.User, userCount, currentUserCount);
        GenerateUsers(userGenerator, Roles.Provider, providerCount, currentProviderCount);
        GenerateUsers(userGenerator, Roles.Admin, adminCount, currentAdminCount);
        
        return app;
    }

    private static int GetCurrentAdminCount(IUserManager userManager)
    {
        int currentAdminCount = userManager.GetUsersAsync(new GetUsersRequest(Roles.Admin)).Result
            .Count(x => x.Email.Contains(Constants.GeneratedEmailSharedKey));
        return currentAdminCount;
    }

    private static int GetCurrentProviderCount(IUserManager userManager)
    {
        int currentProviderCount = userManager.GetUsersAsync(new GetUsersRequest(Roles.Provider)).Result
            .Count(x => x.Email.Contains(Constants.GeneratedEmailSharedKey));
        return currentProviderCount;
    }

    private static int GetCurrentUserCount(IUserManager userManager)
    {
        int currentUserCount = userManager.GetUsersAsync(new GetUsersRequest(Roles.User)).Result
            .Count(x => x.Email.Contains(Constants.GeneratedEmailSharedKey));
        return currentUserCount;
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