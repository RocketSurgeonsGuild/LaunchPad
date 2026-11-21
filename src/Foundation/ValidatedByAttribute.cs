namespace Rocket.Surgery.LaunchPad.Foundation;

/// <summary>
///     Names the validator of the type it marks, for a form validator to create when no validator of the type is registered. The
///     <see cref="MutableAttribute" /> generator puts it on a view model, naming the validator it writes: that validator checks the value the
///     view model makes, so the form binds to the view model and the rules of the type it copies apply.
/// </summary>
/// <param name="validatorType">
///     The validator, a class whose public constructor takes its dependencies from the service provider. Its constructors are kept when the
///     application is trimmed.
/// </param>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
[PublicAPI]
public sealed class ValidatedByAttribute([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type validatorType) : Attribute
{
    /// <summary>
    ///     The validator of the type.
    /// </summary>
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
    public Type ValidatorType { get; } = validatorType;
}
