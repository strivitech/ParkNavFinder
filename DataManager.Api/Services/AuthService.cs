using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;

namespace DataManager.Api.Services;

public class AuthService(IConfiguration configuration) : IAuthService
{
    private readonly IConfiguration _configuration = configuration;

    public async Task<string> GetAccessTokenAsync(string email, string password)
    {
        var client = new AuthenticationApiClient(new Uri(_configuration["Auth0:Authority"]!));

        var request = new ResourceOwnerTokenRequest
        {
            Audience = _configuration["Auth0:Audience"],
            ClientId = _configuration["Auth0:ClientId"],
            Username = email,
            Password = password,
            Scope = "openid profile email"
        };

        var response = await client.GetTokenAsync(request);

        return response.AccessToken;
    }
}