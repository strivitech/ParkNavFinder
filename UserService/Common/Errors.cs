using ErrorOr;

namespace UserService.Common;

public static class Errors
{
    public static class User
    {
        public static Error NotFound() => Error.NotFound("User not found.");
        
        public static Error UpdateFailed() => Error.Failure("Failed to update user.");
    }
}