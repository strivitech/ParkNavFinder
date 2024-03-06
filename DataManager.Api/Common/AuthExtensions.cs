using Auth.Shared;
using DataManager.Api.Services;

namespace DataManager.Api.Common;

public static class AuthExtensions
{
    public static WebApplication SetInMemoryTokensForUsers(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
        var tokenStorage = scope.ServiceProvider.GetRequiredService<ITokenStorage>();
        var usersPool = scope.ServiceProvider.GetRequiredService<IUsersPool>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var password = configuration["Generator:Password"] ??
                       throw new InvalidOperationException("Password not found in configuration.");

        GetAndStoreToken(authService, tokenStorage, usersPool, Roles.User, password);
        GetAndStoreToken(authService, tokenStorage, usersPool, Roles.Provider, password);
        GetAndStoreToken(authService, tokenStorage, usersPool, Roles.Admin, password);

        return app;
    }

    private static void GetAndStoreToken(IAuthService authService, ITokenStorage tokenStorage, IUsersPool usersPool,
        string role, string password)
    {
        var users = usersPool.GetUsers(role);
        foreach (var user in users)
        {
            var token = authService.GetAccessTokenAsync(user.Email, password).Result;
            tokenStorage.StoreToken(user.UserId, token);
        }
    }
}