using FluentValidation;
using ParkingManagementService.Common;

namespace ParkingManagementService.Requests;

public record GetParkingRequest(Guid Id); 

public class GetParkingRequestValidator : AbstractValidator<GetParkingRequest>
{
    public GetParkingRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotDefaultId();
    }
} 