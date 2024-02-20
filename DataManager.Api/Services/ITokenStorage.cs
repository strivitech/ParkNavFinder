namespace DataManager.Api.Services;

public interface ITokenStorage
{
    void StoreToken(string userId, string token);

    string GetToken(string userId);
}