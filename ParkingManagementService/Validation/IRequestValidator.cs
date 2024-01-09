using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using ErrorOr;

namespace ParkingManagementService.Validation;

public interface IRequestValidator
{
    List<Error> Validate<T>(    
        [NotNull] T model,
        [CallerArgumentExpression("model")] string? paramName = null);
    
    bool CheckIfValid<T>([NotNull]T model, [CallerArgumentExpression("model")] string? paramName = null);
}