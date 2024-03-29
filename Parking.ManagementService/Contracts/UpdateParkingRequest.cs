﻿using FluentValidation;
using Parking.ManagementService.Domain;
using Parking.ManagementService.Validation;

namespace Parking.ManagementService.Contracts;

public record UpdateParkingRequest( 
    Guid Id,
    string Name,
    string Description,
    Address Address,
    int TotalSpaces);
    
public class UpdateParkingRequestValidator : AbstractValidator<UpdateParkingRequest>
{
    public UpdateParkingRequestValidator(IValidator<Address> addressValidator)
    {
        RuleFor(x => x.Id)
            .NotDefaultId();
        
        RuleFor(x => x.Name)
            .MinimumLength(1)
            .MaximumLength(100);
        
        RuleFor(x => x.Description)
            .MinimumLength(1)
            .MaximumLength(2000);

        RuleFor(x => x.Address)
            .SetValidator(addressValidator);
        
        RuleFor(x => x.TotalSpaces)
            .GreaterThan(0)
            .LessThan(10000);
    }
}