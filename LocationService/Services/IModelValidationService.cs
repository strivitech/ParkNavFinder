using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using LocationService.Common;

namespace LocationService.Services;

/// <summary>
/// Validates input models.
/// </summary>
public interface IModelValidationService
{
    /// <summary>
    /// Checks whether the model is valid or not.
    /// </summary>
    /// <param name="model">Model to validate.</param>
    /// <param name="paramName">Parameter name.</param>
    /// <typeparam name="T">The type of model.</typeparam>
    /// <returns>The result contains true if valid; otherwise, false.</returns>
    
    bool CheckIfValid<T>([NotNull]T model, [CallerArgumentExpression("model")] string? paramName = null);

    /// <summary>
    /// Throws <see cref="ModelValidationException"/> if the model is not valid.
    /// </summary>
    /// <param name="model">Model to validate.</param>
    /// <param name="paramName">Parameter name.</param>
    /// <typeparam name="T">The type of model.</typeparam>
    void ThrowIfNotValid<T>([NotNull] T model, [CallerArgumentExpression("model")] string? paramName = null);
}