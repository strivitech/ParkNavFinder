using FluentValidation;
using Parking.ManagementService.Validation;

namespace Parking.ManagementService.Contracts;

public record GetParkingRequest(Guid Id); 

public class GetParkingRequestValidator : AbstractValidator<GetParkingRequest>
{
    public GetParkingRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotDefaultId();
    }
} 