using FairyBread;

namespace Rocket.Surgery.LaunchPad.HotChocolate.Validation;

/// <summary>
///     Maintains a registry of implicit validators
///     keyed by the target runtime type for validation.
/// </summary>
public interface ICustomValidatorRegistry
{
    /// <summary>
    ///     Try and get the validator descriptor for the specified type.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="descriptor"></param>
    /// <returns></returns>
    bool TryGetValidator(Type type, [NotNullWhen(true)] out ValidatorDescriptor? descriptor);
}
