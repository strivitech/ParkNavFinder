using FluentValidation;
using ParkingManagementService.Common;
using ParkingManagementService.Validation;

namespace ParkingManagementService.Parking;

public record DeleteParkingRequest(Guid Id);

public class DeleteParkingRequestValidator : AbstractValidator<DeleteParkingRequest>
{
    public DeleteParkingRequestValidator(IValidator<Address> addressValidator)
    {
        RuleFor(x => x.Id)
            .NotDefaultId();
    }
}