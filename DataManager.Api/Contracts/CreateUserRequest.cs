using Auth.Shared;
using FluentValidation;

namespace DataManager.Api.Contracts;

public record CreateUserRequest(string Email, string Password, string Role);

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    private static readonly List<string> ValidRoles = [Roles.User, Roles.Provider, Roles.Admin];

    public CreateUserRequestValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress();

        RuleFor(x => x.Password)
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*]).{8,}$")
            .WithMessage(
                "Password must contain at least 8 characters, one uppercase letter, one lowercase letter, one number, and one special character");

        RuleFor(x => x.Role)
            .Must(role => ValidRoles.Contains(role))
            .WithMessage("Invalid role. The role must be one of the following: User, Provider, Admin");
    }
}