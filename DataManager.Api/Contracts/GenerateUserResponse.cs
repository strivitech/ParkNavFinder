namespace DataManager.Api.Contracts;

public record GenerateUserResponse(string UserId, string Email, string Password, string Role);