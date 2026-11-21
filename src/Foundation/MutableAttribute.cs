using System.Diagnostics;

namespace Rocket.Surgery.LaunchPad.Foundation;

/// <summary>
///     Makes the partial class it marks a mutable view model of another type: the generator gives the class a settable property for each of
///     that type's properties, <c>From(value)</c> to copy a value into it, <c>ToImmutable()</c> to make the value from it and an implicit
///     conversion each way. The copied type can be a class, a record, a record struct or a readonly record struct, in this assembly or another
///     one. The class and the types that contain it must be partial.
/// </summary>
/// <remarks>
///     A Blazor <c>EditForm</c> binds to the view model, which an immutable request can't be bound to. In a project that references
///     ZeroAlloc.Validation the generator also writes a nested <c>Validator</c>, a <c>ValidatorFor</c> of the view model that validates the
///     value it makes with that type's validator, and names it with <see cref="ValidatedByAttribute" />, so LaunchPad's
///     <c>ZeroAllocValidator</c> applies the copied type's rules to the form. A property the class declares itself is not generated, and
///     <c>From</c> and <c>ToImmutable()</c> use it.
/// </remarks>
/// <param name="type">The type the class is a mutable view model of.</param>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
[PublicAPI]
[Conditional("CodeGeneration")]
public sealed class MutableAttribute(Type type) : Attribute
{
    /// <summary>
    ///     The type the class is a mutable view model of.
    /// </summary>
    public Type Type { get; } = type;
}

/// <summary>
///     Makes the partial class it marks a mutable view model of <typeparamref name="T" />: the generator gives the class a settable property for
///     each of its properties, <c>From(value)</c> to copy a value into it, <c>ToImmutable()</c> to make the value from it and an implicit
///     conversion each way. <typeparamref name="T" /> can be a class, a record, a record struct or a readonly record struct, in this assembly or
///     another one. The class and the types that contain it must be partial.
/// </summary>
/// <remarks>
///     A Blazor <c>EditForm</c> binds to the view model, which an immutable request can't be bound to. In a project that references
///     ZeroAlloc.Validation the generator also writes a nested <c>Validator</c>, a <c>ValidatorFor</c> of the view model that validates the
///     value it makes with that type's validator, and names it with <see cref="ValidatedByAttribute" />, so LaunchPad's
///     <c>ZeroAllocValidator</c> applies the copied type's rules to the form. A property the class declares itself is not generated, and
///     <c>From</c> and <c>ToImmutable()</c> use it.
/// </remarks>
/// <typeparam name="T">The type the class is a mutable view model of.</typeparam>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
[PublicAPI]
[Conditional("CodeGeneration")]
public sealed class MutableAttribute<T> : Attribute where T : notnull;
