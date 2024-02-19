using DataManager.Api.Contracts;

namespace DataManager.Api.Services;

public class UserGenerator(
    IUserManager userManager,
    IEmailGenerator emailGenerator,
    IConfiguration configuration) : IUserGenerator
{
    private readonly IUserManager _userManager = userManager;
    private readonly IEmailGenerator _emailGenerator = emailGenerator;
    private readonly IConfiguration _configuration = configuration;

    public async Task<List<GenerateUserResponse>> GenerateAsync(string role, int count)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(count, 1);
        var responses = new List<GenerateUserResponse>();

        for (int i = 0; i < count; i++)
        {
            var email = _emailGenerator.Generate(RetrieveEmailValidPart());
            var password = RetrievePassword();
            var createUserResponse = await _userManager.CreateUserAsync(new CreateUserRequest(email, password, role));
            responses.Add(new GenerateUserResponse(createUserResponse.UserId, email, password, role));
        }
        
        return responses;
    }

    private string RetrieveEmailValidPart() =>
        _configuration["Generator:EmailValidPart"] ??
        throw new InvalidOperationException("Email valid part is not set in the configuration.");
    
    private string RetrievePassword() =>
        _configuration["Generator:Password"] ??
        throw new InvalidOperationException("Password is not set in the configuration.");
}