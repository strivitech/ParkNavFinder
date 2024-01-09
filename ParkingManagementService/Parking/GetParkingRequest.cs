using FluentValidation;
using ParkingManagementService.Common;
using ParkingManagementService.Validation;

namespace ParkingManagementService.Parking;

public record GetParkingRequest(Guid Id); 

public class GetParkingRequestValidator : AbstractValidator<GetParkingRequest>
{
    public GetParkingRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotDefaultId();
    }
} 