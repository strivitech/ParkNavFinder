using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace LocationService.Validation;

public interface IRequestValidator
{
    bool CheckIfValid<T>([NotNull]T model, [CallerArgumentExpression("model")] string? paramName = null);
    
    void ThrowIfNotValid<T>([NotNull] T model, [CallerArgumentExpression("model")] string? paramName = null);
}