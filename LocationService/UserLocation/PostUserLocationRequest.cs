using FluentValidation;

namespace LocationService.UserLocation;

public record PostUserLocationRequest(string UserId, double Latitude, double Longitude, DateTime Timestamp);

public class PostUserLocationRequestValidator : AbstractValidator<PostUserLocationRequest>
{
    public PostUserLocationRequestValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Latitude).InclusiveBetween(-90, 90);
        RuleFor(x => x.Longitude).InclusiveBetween(-180, 180);
        RuleFor(x => x.Timestamp)
            .NotEmpty()
            .Must(BeAValidDate).WithMessage("Timestamp must be a valid date")
            .Must(BeAPastOrPresentDate).WithMessage("Timestamp must be in the past or present");
    }
    
    private static bool BeAValidDate(DateTime date) => date != default;

    private static bool BeAPastOrPresentDate(DateTime date) => date <= DateTime.UtcNow;
}