using DataManager.Api.Contracts;
using GetUsersRequest = DataManager.Api.Contracts.GetUsersRequest;

namespace DataManager.Api.Services;

public interface IUserManager
{
    Task<List<GetUserResponse>> GetUsersAsync(GetUsersRequest request);
    
    Task<string> GetUserRoleAsync(GetUserRoleRequest request);
    
    Task<CreateUserResponse> CreateUserAsync(CreateUserRequest request);        

    Task DeleteUserAsync(DeleteUserRequest request);
}