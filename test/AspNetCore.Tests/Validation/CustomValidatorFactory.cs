using FluentValidation;

namespace AspNetCore.Tests.Validation;

/// <summary>
///     <see cref="IValidatorFactory" /> that works with registered validators.
/// </summary>
public class CustomValidatorFactory : IValidatorFactory
{
    private readonly List<IValidator> _validators;

    public CustomValidatorFactory(IEnumerable<IValidator> validators)
    {
        _validators = validators.ToList();
    }

    /// <inheritdoc />
    public IValidator<T> GetValidator<T>()
    {
        return _validators.FirstOrDefault(validator => validator.CanValidateInstancesOfType(typeof(T))) as IValidator<T>;
    }

    /// <inheritdoc />
    public IValidator GetValidator(Type type)
    {
        return _validators.FirstOrDefault(validator => validator.CanValidateInstancesOfType(type));
    }
}
