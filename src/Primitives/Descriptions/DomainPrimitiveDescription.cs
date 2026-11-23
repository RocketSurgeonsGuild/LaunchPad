namespace Rocket.Surgery.LaunchPad.Primitives.Descriptions;

/// <summary>
///     What an integration needs to know about one domain primitive that holds one value: its value type, the format it is written in, and
///     the constraints of its rules (lengths, patterns, bounds, allowed values). A rule written as code (<c>[CustomValidator&lt;T&gt;]</c>) is
///     not described.
/// </summary>
/// <remarks>
///     The descriptions of an application's domain primitives are written by the Primitives generator, one closed generic per domain
///     primitive, from the rules it read at compile time, so nothing here reflects over the types. <see cref="Accept{TResult}" /> gives an
///     integration the domain primitive and its value type as generic arguments.
/// </remarks>
[PublicAPI]
public abstract class DomainPrimitiveDescription
{
    /// <summary>
    ///     Describes a domain primitive.
    /// </summary>
    /// <typeparam name="TPrimitive">The domain primitive.</typeparam>
    /// <typeparam name="TValue">Its value.</typeparam>
    /// <returns>The description.</returns>
    public static DomainPrimitiveDescription<TPrimitive, TValue> Create<TPrimitive, TValue>()
        where TPrimitive : IDomainPrimitive<TPrimitive, TValue>
        where TValue : notnull => new();

    private protected DomainPrimitiveDescription() { }

    /// <summary>
    ///     The domain primitive.
    /// </summary>
    public abstract Type Type { get; }

    /// <summary>
    ///     The value the domain primitive holds, which is what it is written as.
    /// </summary>
    public abstract Type ValueType { get; }

    /// <summary>
    ///     Whether the domain primitive is a strong ID.
    /// </summary>
    public bool IsStrongId { get; init; }

    /// <summary>
    ///     Whether the domain primitive is a smart enum, whose <see cref="AllowedValues" /> are its names.
    /// </summary>
    public bool IsSmartEnum { get; init; }

    /// <summary>
    ///     The format of the value (an OpenAPI / JSON Schema format such as <c>email</c> or <c>uri</c>), when its kind has one.
    /// </summary>
    public string? Format { get; init; }

    /// <summary>
    ///     The shortest a text value may be.
    /// </summary>
    public int? MinLength { get; init; }

    /// <summary>
    ///     The longest a text value may be.
    /// </summary>
    public int? MaxLength { get; init; }

    /// <summary>
    ///     The regular expressions a text value must match, every one of them.
    /// </summary>
    public IReadOnlyList<string> Patterns { get; init; } = [];

    /// <summary>
    ///     The least a value may be, written in the invariant culture.
    /// </summary>
    public string? Minimum { get; init; }

    /// <summary>
    ///     Whether <see cref="Minimum" /> itself is excluded.
    /// </summary>
    public bool MinimumIsExclusive { get; init; }

    /// <summary>
    ///     The most a value may be, written in the invariant culture.
    /// </summary>
    public string? Maximum { get; init; }

    /// <summary>
    ///     Whether <see cref="Maximum" /> itself is excluded.
    /// </summary>
    public bool MaximumIsExclusive { get; init; }

    /// <summary>
    ///     The only values allowed, in ordinal order, when the rules list them (a smart enum's names).
    /// </summary>
    public IReadOnlyList<string>? AllowedValues { get; init; }

    /// <summary>
    ///     The number of digits after the decimal point a decimal value keeps, when its kind sets one.
    /// </summary>
    public int? Scale { get; init; }

    /// <summary>
    ///     Hands the domain primitive and its value to <paramref name="visitor" /> as generic arguments.
    /// </summary>
    /// <param name="visitor">The visitor.</param>
    /// <typeparam name="TResult">What the visitor returns.</typeparam>
    /// <returns>What the visitor returned.</returns>
    public abstract TResult Accept<TResult>(IDomainPrimitiveVisitor<TResult> visitor);
}

/// <summary>
///     The description of one domain primitive, which knows the domain primitive and its value as generic arguments.
/// </summary>
/// <typeparam name="TPrimitive">The domain primitive.</typeparam>
/// <typeparam name="TValue">Its value.</typeparam>
[PublicAPI]
public sealed class DomainPrimitiveDescription<TPrimitive, TValue> : DomainPrimitiveDescription
    where TPrimitive : IDomainPrimitive<TPrimitive, TValue>
    where TValue : notnull
{
    internal DomainPrimitiveDescription() { }

    /// <inheritdoc />
    public override Type Type => typeof(TPrimitive);

    /// <inheritdoc />
    public override Type ValueType => typeof(TValue);

    /// <inheritdoc />
    public override TResult Accept<TResult>(IDomainPrimitiveVisitor<TResult> visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        return visitor.Visit(this);
    }
}

/// <summary>
///     Does something with a domain primitive known by its generic arguments, such as building a converter for it.
/// </summary>
/// <typeparam name="TResult">What the visitor returns.</typeparam>
[PublicAPI]
public interface IDomainPrimitiveVisitor<out TResult>
{
    /// <summary>
    ///     Visits one domain primitive.
    /// </summary>
    /// <param name="description">Its description.</param>
    /// <typeparam name="TPrimitive">The domain primitive.</typeparam>
    /// <typeparam name="TValue">Its value.</typeparam>
    /// <returns>The result.</returns>
    TResult Visit<TPrimitive, TValue>(DomainPrimitiveDescription<TPrimitive, TValue> description)
        where TPrimitive : IDomainPrimitive<TPrimitive, TValue>
        where TValue : notnull;
}

/// <summary>
///     What an integration needs to know about one value object: the type and the values it holds, in declaration order.
/// </summary>
/// <param name="Type">The value object.</param>
/// <param name="Properties">The names of the values it holds.</param>
[PublicAPI]
public sealed record ValueObjectDescription(Type Type, IReadOnlyList<string> Properties);
