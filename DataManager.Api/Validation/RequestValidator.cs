using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using FluentValidation;
using FluentValidation.Results;

namespace DataManager.Api.Validation;

public sealed class RequestValidator(IServiceProvider serviceProvider)
    : IRequestValidator
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public void ThrowIfNotValid<T>([NotNull]T model, [CallerArgumentExpression("model")] string? paramName = null)
    {   
        ArgumentNullException.ThrowIfNull(model, paramName);

        var validationResult = Validate(model);
        
        if (!validationResult.IsValid)
        {
            throw new RequestValidationException(validationResult.Errors);
        }
    }
    
    public bool CheckIfValid<T>([NotNull]T model, [CallerArgumentExpression("model")] string? paramName = null)
    {
        ArgumentNullException.ThrowIfNull(model, paramName);

        var validationResult = Validate(model);
        return validationResult.IsValid;
    }

    private ValidationResult Validate<T>(T model)
    {
        var validator = GetValidator<T>();

        return validator.Validate(model);
    }
    
    private IValidator<T> GetValidator<T>()
    {
        var validator = _serviceProvider.GetService<IValidator<T>>();

        return validator ?? throw new InvalidOperationException($"Validator for type {typeof(T).FullName} was not found. Check if it's already registered.");
    }
}