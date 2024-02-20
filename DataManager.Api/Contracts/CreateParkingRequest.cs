using FluentValidation;

namespace DataManager.Api.Contracts;

public record CreateParkingRequest(
    string Name,
    string Description,
    Address Address,
    double Latitude,
    double Longitude,
    int TotalSpaces);

public class CreateParkingRequestValidator : AbstractValidator<CreateParkingRequest>
{
    public CreateParkingRequestValidator(IValidator<Address> addressValidator)
    {
        RuleFor(x => x.Name)
            .MinimumLength(1)
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MinimumLength(1)
            .MaximumLength(2000);

        RuleFor(x => x.Address)
            .SetValidator(addressValidator);

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90);

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180);

        RuleFor(x => x.TotalSpaces)
            .GreaterThan(0)
            .LessThan(10000);
    }
}