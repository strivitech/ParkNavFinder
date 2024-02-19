using DataManager.Api.Contracts;

namespace DataManager.Api.Common;

public static class RequestExtensions
{
    public static string GetUniqueIdentifier(this GetUsersRequest request) => request.Role;
}