using FluentValidation;

namespace User.WebSocketHandler.Contracts;

public record PostUserLocationRequest(string UserId, double Latitude, double Longitude);

public class PostUserLocationRequestValidator : AbstractValidator<PostUserLocationRequest>
{
    public PostUserLocationRequestValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Latitude).InclusiveBetween(-90, 90);
        RuleFor(x => x.Longitude).InclusiveBetween(-180, 180);
    }
}