using FluentValidation.Results;

namespace LocationService.Common;

/// <summary>
/// An exception if a model isn't valid.
/// </summary>
[Serializable]
public class ModelValidationException : Exception
{
    /// <summary>
    /// Initializes a new instance of <see cref="ModelValidationException"/>.
    /// </summary>
    public ModelValidationException()
        : base("One or more validation failures have occurred.")
    {
        Errors = new Dictionary<string, string[]>();
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ModelValidationException"/>.
    /// </summary>
    /// <param name="failures">Validation failures.</param>
    public ModelValidationException(IEnumerable<ValidationFailure> failures)
        : this()
    {
        Errors = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
    }

    /// <summary>
    /// Contains validation errors.
    /// </summary>
    public IDictionary<string, string[]> Errors { get; }
}