using Auth.Shared;
using FluentValidation;

namespace DataManager.Api.Contracts;

public record GetUsersRequest(string Role);

public class GetUsersRequestValidator : AbstractValidator<GetUsersRequest>
{
    private static readonly List<string> ValidRoles = [Roles.User, Roles.Provider, Roles.Admin];
    
    public GetUsersRequestValidator()
    {
        RuleFor(x => x.Role)
            .Must(role => ValidRoles.Contains(role))
            .WithMessage("Invalid role. The role must be one of the following: User, Provider, Admin");
    }
}