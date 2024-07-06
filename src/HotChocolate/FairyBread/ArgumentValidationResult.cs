using FluentValidation;
using FluentValidation.Results;

namespace Rocket.Surgery.LaunchPad.HotChocolate.FairyBread;

public record ArgumentValidationResult
{
    public ArgumentValidationResult(
        string argumentName,
        IValidator validator,
        ValidationResult result
    )
    {
        ArgumentName = argumentName;
        Validator = validator;
        Result = result ?? throw new ArgumentNullException(nameof(result));
    }

    /// <summary>
    ///     Name of the argument this result is for.
    /// </summary>
    public string ArgumentName { get; }

    /// <summary>
    ///     The validator that caused this result.
    /// </summary>
    public IValidator Validator { get; }

    /// <summary>
    ///     The validation result.
    /// </summary>
    public ValidationResult Result { get; }
}