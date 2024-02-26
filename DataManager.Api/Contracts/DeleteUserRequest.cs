using FluentValidation;

namespace DataManager.Api.Contracts;

public record DeleteUserRequest(string UserId);

public class DeleteUserRequestValidator : AbstractValidator<DeleteUserRequest>
{
    public DeleteUserRequestValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}