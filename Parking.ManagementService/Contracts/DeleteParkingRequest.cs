using FluentValidation;
using Parking.ManagementService.Domain;
using Parking.ManagementService.Validation;

namespace Parking.ManagementService.Contracts;

public record DeleteParkingRequest(Guid Id);

public class DeleteParkingRequestValidator : AbstractValidator<DeleteParkingRequest>
{
    public DeleteParkingRequestValidator(IValidator<Address> addressValidator)
    {
        RuleFor(x => x.Id)
            .NotDefaultId();
    }
}