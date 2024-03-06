using DataManager.Api.Contracts;

namespace DataManager.Api.Services;

public interface IUsersPool
{
    void SetUsers(string role, List<GetUserResponse> users);

    List<GetUserResponse> GetUsers(string role);    
}