using FluentValidation;
using ParkingManagementService.Common;
using ParkingManagementService.Models;

namespace ParkingManagementService.Requests;

public record DeleteParkingRequest(Guid Id);

public class DeleteParkingRequestValidator : AbstractValidator<DeleteParkingRequest>
{
    public DeleteParkingRequestValidator(IValidator<Address> addressValidator)
    {
        RuleFor(x => x.Id)
            .NotDefaultId();
    }
}