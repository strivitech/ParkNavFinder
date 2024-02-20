using Auth.Shared;
using DataManager.Api.Contracts;
using DataManager.Api.Services;

namespace DataManager.Api.Common;

public static class AuthExtensions
{
    public static WebApplication SetInMemoryTokensForUsers(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
        var tokenStorage = scope.ServiceProvider.GetRequiredService<ITokenStorage>();
        var userManager = scope.ServiceProvider.GetRequiredService<IUserManager>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var password = configuration["Generator:Password"] ??
                       throw new InvalidOperationException("Password not found in configuration.");

        GetAndStoreToken(authService, tokenStorage, userManager, Roles.User, password);
        GetAndStoreToken(authService, tokenStorage, userManager, Roles.Provider, password);
        GetAndStoreToken(authService, tokenStorage, userManager, Roles.Admin, password);

        return app;
    }

    private static void GetAndStoreToken(IAuthService authService, ITokenStorage tokenStorage, IUserManager userManager,
        string role, string password)
    {
        var users = userManager.GetUsersAsync(new GetUsersRequest(role)).Result
            .Where(u => u.Email.Contains(Constants.GeneratedEmailSharedKey));
        foreach (var user in users)
        {
            var token = authService.GetAccessTokenAsync(user.Email, password).Result;
            tokenStorage.StoreToken(user.UserId, token);
        }
    }
}