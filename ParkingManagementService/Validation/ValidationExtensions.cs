using FluentValidation;

namespace ParkingManagementService.Validation;

internal static class ValidationExtensions
{
    public static IRuleBuilderOptions<T, Guid> NotDefaultId<T>(this IRuleBuilder<T, Guid> ruleBuilder)
    {
        return ruleBuilder.Must(id => id != Guid.Empty).WithMessage("Id must not be the default value.");
    }
}