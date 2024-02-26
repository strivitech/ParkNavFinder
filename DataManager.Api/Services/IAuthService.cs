namespace DataManager.Api.Services;

public interface IAuthService
{
    Task<string> GetAccessTokenAsync(string email, string password);
}