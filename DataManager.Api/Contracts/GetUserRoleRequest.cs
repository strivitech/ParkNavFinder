using FluentValidation;

namespace DataManager.Api.Contracts;

public record GetUserRoleRequest(string UserId);

public class GetUserRoleRequestValidator : AbstractValidator<GetUserRoleRequest>
{
    public GetUserRoleRequestValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}