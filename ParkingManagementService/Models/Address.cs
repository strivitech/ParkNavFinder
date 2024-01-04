using FluentValidation;

namespace ParkingManagementService.Models;

public record Address(
    string Country,
    string City,
    string Street,
    string StreetNumber);
    
public class AddressValidator : AbstractValidator<Address>
{
    public AddressValidator()
    {
        RuleFor(x => x.Country)
            .NotEmpty()
            .MaximumLength(100);
        
        RuleFor(x => x.City)
            .NotEmpty()
            .MaximumLength(100);
        
        RuleFor(x => x.Street)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.StreetNumber)
            .NotEmpty()
            .MaximumLength(10);
    }
}