using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using DataManager.Api.Contracts;
using DataManager.Api.Validation;
using GetUsersRequest = DataManager.Api.Contracts.GetUsersRequest;

namespace DataManager.Api.Services;

public class UserManager(ManagementApiClient managementApiClient, IRequestValidator requestValidator) : IUserManager
{
    private readonly ManagementApiClient _managementApiClient = managementApiClient;
    private readonly IRequestValidator _requestValidator = requestValidator;

    public async Task<List<GetUserResponse>> GetUsersAsync(GetUsersRequest request)
    {
        _requestValidator.ThrowIfNotValid(request);
        
        var getRolesRequest = new GetRolesRequest { NameFilter = request.Role };
        var roles = await _managementApiClient.Roles.GetAllAsync(getRolesRequest);
        var roleId = roles.Single().Id;
        var users = await _managementApiClient.Roles.GetUsersAsync(roleId);
        
        return users.Select(user => new GetUserResponse(user.UserId, user.Email, request.Role))
            .ToList();
    }

    public async Task<string> GetUserRoleAsync(GetUserRoleRequest request)
    {
        _requestValidator.ThrowIfNotValid(request);
        
        var roles = await _managementApiClient.Users.GetRolesAsync(request.UserId);
        return roles.Single().Name;
    }

    public async Task<CreateUserResponse> CreateUserAsync(CreateUserRequest request)
    {
        _requestValidator.ThrowIfNotValid(request);
        
        var userRequest = new UserCreateRequest
        {
            Email = request.Email,
            Password = request.Password,
            Connection = "Username-Password-Authentication",
        };

        var user = await _managementApiClient.Users.CreateAsync(userRequest);
        var getRolesRequest = new GetRolesRequest { NameFilter = request.Role };
        var roles = await _managementApiClient.Roles.GetAllAsync(getRolesRequest);
        var roleId = roles.Single().Id;
        await _managementApiClient.Roles.AssignUsersAsync(roleId,
            new AssignUsersRequest { Users = [user.UserId] });

        return new CreateUserResponse(user.UserId);
    }

    public async Task DeleteUserAsync(DeleteUserRequest request) => await _managementApiClient.Users.DeleteAsync(request.UserId);
}