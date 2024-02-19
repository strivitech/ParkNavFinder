using DataManager.Api.Contracts;

namespace DataManager.Api.Services;

public interface IUserGenerator
{
    public Task<List<GenerateUserResponse>> GenerateAsync(string role, int count);
}