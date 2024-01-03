using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using ErrorOr;
using FluentValidation;
using FluentValidation.Results;

namespace ParkingManagementService.Services;

/// <summary>
/// Validates input models using FluentValidation.
/// </summary>
public sealed class ModelValidationService : IModelValidationService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ModelValidationService> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="ModelValidationService"/>.
    /// </summary>
    /// <param name="serviceProvider">Service provider.</param>
    /// <param name="logger">Logger.</param>
    /// <exception cref="ArgumentNullException">If some parameter contains null.</exception>
    public ModelValidationService(IServiceProvider serviceProvider, ILogger<ModelValidationService> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger;
    }

    public List<Error> Validate<T>([NotNull]T model, [CallerArgumentExpression("model")] string? paramName = null)
    {
        ArgumentNullException.ThrowIfNull(model, paramName);

        var validationResult = DoValidation(model);
        
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(x => Error.Validation(x.PropertyName, x.ErrorMessage)).ToList();
            _logger.LogDebug("Model is invalid: {Model}", model);
            _logger.LogDebug("Validation errors: {Errors}", errors);
        }

        return [];
    }

    public bool CheckIfValid<T>([NotNull]T model, [CallerArgumentExpression("model")] string? paramName = null)
    {
        ArgumentNullException.ThrowIfNull(model, paramName);

        var validationResult = DoValidation(model);

        if (!validationResult.IsValid)
        {
            _logger.LogDebug("Model is invalid: {Model}", model);
            _logger.LogDebug("Validation errors: {Errors}", validationResult.Errors.Select(x => x.ErrorMessage));
        }
        
        return validationResult.IsValid;
    }

    private ValidationResult DoValidation<T>(T model)
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