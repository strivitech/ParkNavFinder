using ErrorOr;
using UserService.Responses;

namespace UserService.Services;

public interface IUserService
{
    Task<ErrorOr<GetUserResponse>> GetByIdAsync(string userId);
    
    Task<ErrorOr<Updated>> UpdateAsync(string userId, UpdateUserRequest request);
}

public class UpdateUserRequest
{
    public string? PhoneNumber { get; set; }
}