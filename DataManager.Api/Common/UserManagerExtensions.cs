using DataManager.Api.Contracts;
using DataManager.Api.Services;

namespace DataManager.Api.Common;

public static class UserManagerExtensions
{
    public static async Task<List<GetUserResponse>> GetGeneratedUsersAsync(this IUserManager userManager, string role)
    {
        return (await userManager.GetUsersAsync(new GetUsersRequest(role)))
            .Where(x => x.Email.Contains(Constants.GeneratedEmailSharedKey))
            .ToList();
    }
    
    public static async Task<int> CountGeneratedUsersAsync(this IUserManager userManager, string role)
    {
        return (await userManager.GetUsersAsync(new GetUsersRequest(role)))
            .Count(x => x.Email.Contains(Constants.GeneratedEmailSharedKey));
    }
}