using System.Diagnostics;

namespace Rocket.Surgery.LaunchPad.Foundation;

/// <summary>
///     Generates a mutable view model of the type, a class next to it named <c>{Prefix}{Name}{Suffix}</c> (<c>RequestViewModel</c> by default):
///     a settable property for each of the type's properties, <c>From(value)</c> to copy a value into it and <c>ToImmutable()</c> to make the
///     value from it (<see cref="IMutable{T}" />). The type can be a class, a record, a record struct or a readonly record struct, and the types
///     that contain it must be partial.
/// </summary>
/// <remarks>
///     A Blazor <c>EditForm</c> binds to the view model, which an immutable request can't be bound to. LaunchPad's <c>ZeroAllocValidator</c>
///     validates the view model with the validator of the type it makes.
/// </remarks>
/// <remarks>
///     Generate a mutable view model of the type.
/// </remarks>
/// <param name="prefix">What goes before the type's name in the view model's name.</param>
/// <param name="suffix">What goes after the type's name in the view model's name.</param>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = true)]
[Conditional("CodeGeneration")]
public sealed class MutableAttribute(string prefix = "", string suffix = "ViewModel") : Attribute
{

    /// <summary>
    ///     What goes before the type's name in the view model's name.
    /// </summary>
    public string Prefix { get; } = prefix;

    /// <summary>
    ///     What goes after the type's name in the view model's name.
    /// </summary>
    public string Suffix { get; } = suffix;
}
