using System.Globalization;
using System.Numerics;

using SuperStrong.Types;
using SuperStrong.Types.Validators;

namespace Rocket.Surgery.LaunchPad.StrongTypes;

/// <summary>
///     What an integration needs to know about one strong type: its value type, the format it is written in, and the constraints its rules
///     set (lengths, patterns, bounds, allowed values), read from its SuperStrong validators. A rule written as a predicate
///     (<c>Satisfies</c>) is not described.
/// </summary>
/// <remarks>
///     The descriptions of an application's strong types are written by Primitives' source generator, one closed generic per strong type,
///     so nothing here reflects over the types. <see cref="Accept{TResult}" /> gives an integration the strong type and its value type as
///     generic arguments.
/// </remarks>
[PublicAPI]
public abstract class StrongTypeDescription
{
    private protected StrongTypeDescription(string? format) => Format = format;

    /// <summary>
    ///     Describes a strong type whose value is not a number.
    /// </summary>
    /// <param name="format">The format of the value, from <see cref="StrongTypeFormatAttribute" />.</param>
    /// <typeparam name="TStrongType">The strong type.</typeparam>
    /// <typeparam name="TPrimitive">Its value.</typeparam>
    /// <returns>The description.</returns>
    public static StrongTypeDescription Create<TStrongType, TPrimitive>(string? format = null)
        where TStrongType : class, IStrongType<TStrongType, TPrimitive>
        where TPrimitive : notnull
    {
        var description = new StrongTypeDescription<TStrongType, TPrimitive>(format);
        description.Read(TStrongType.Define());
        return description;
    }

    /// <summary>
    ///     Describes a strong type whose value is a number, which also has bounds.
    /// </summary>
    /// <param name="format">The format of the value, from <see cref="StrongTypeFormatAttribute" />.</param>
    /// <typeparam name="TStrongType">The strong type.</typeparam>
    /// <typeparam name="TPrimitive">Its value.</typeparam>
    /// <returns>The description.</returns>
    public static StrongTypeDescription CreateNumber<TStrongType, TPrimitive>(string? format = null)
        where TStrongType : class, IStrongType<TStrongType, TPrimitive>
        where TPrimitive : INumberBase<TPrimitive>, IComparable<TPrimitive>
    {
        var description = new StrongTypeDescription<TStrongType, TPrimitive>(format);
        var definition = TStrongType.Define();
        description.Read(definition);
        foreach (var validator in definition.Validators)
        {
            switch (validator)
            {
                case MinValueValidator<TPrimitive> min:
                    description.SetMinimum(Text(min.MinValue), min.IsExclusive);
                    break;
                case MaxValueValidator<TPrimitive> max:
                    description.SetMaximum(Text(max.MaxValue), max.IsExclusive);
                    break;
                case PositiveValidator<TPrimitive> positive:
                    description.SetMinimum("0", !positive.AllowZero);
                    break;
                case NegativeValidator<TPrimitive> negative:
                    description.SetMaximum("0", !negative.AllowZero);
                    break;
                default:
                    break;
            }
        }

        return description;
    }

    /// <summary>
    ///     The strong type.
    /// </summary>
    public abstract Type Type { get; }

    /// <summary>
    ///     The value the strong type wraps, which is what it is written as.
    /// </summary>
    public abstract Type PrimitiveType { get; }

    /// <summary>
    ///     The format of the value (an OpenAPI / JSON Schema format such as <c>email</c> or <c>uri</c>), when the strong type or its rules
    ///     declare one with <see cref="StrongTypeFormatAttribute" />.
    /// </summary>
    public string? Format { get; }

    /// <summary>
    ///     The shortest a text value may be.
    /// </summary>
    public int? MinLength { get; private set; }

    /// <summary>
    ///     The longest a text value may be.
    /// </summary>
    public int? MaxLength { get; private set; }

    /// <summary>
    ///     The regular expressions a text value must match, every one of them.
    /// </summary>
    public IReadOnlyList<string> Patterns => _patterns;

    /// <summary>
    ///     The least a number may be, written in the invariant culture.
    /// </summary>
    public string? Minimum { get; private set; }

    /// <summary>
    ///     Whether <see cref="Minimum" /> itself is excluded.
    /// </summary>
    public bool MinimumIsExclusive { get; private set; }

    /// <summary>
    ///     The most a number may be, written in the invariant culture.
    /// </summary>
    public string? Maximum { get; private set; }

    /// <summary>
    ///     Whether <see cref="Maximum" /> itself is excluded.
    /// </summary>
    public bool MaximumIsExclusive { get; private set; }

    /// <summary>
    ///     The only values allowed, written in the invariant culture, when the rules list them.
    /// </summary>
    public IReadOnlyList<string>? AllowedValues { get; private set; }

    /// <summary>
    ///     Hands the strong type and its value to <paramref name="visitor" /> as generic arguments.
    /// </summary>
    /// <param name="visitor">The visitor.</param>
    /// <typeparam name="TResult">What the visitor returns.</typeparam>
    /// <returns>What the visitor returned.</returns>
    public abstract TResult Accept<TResult>(IStrongTypeVisitor<TResult> visitor);

    private protected void Read<TPrimitive>(StrongTypeDefinition<TPrimitive> definition) where TPrimitive : notnull
    {
        foreach (var validator in definition.Validators)
        {
            switch (validator)
            {
                case MinLengthValidator min:
                    MinLength = Math.Max(MinLength ?? 0, min.MinLength);
                    break;
                case MaxLengthValidator max:
                    MaxLength = Math.Min(MaxLength ?? int.MaxValue, max.MaxLength);
                    break;
                case ExactLengthValidator exact:
                    MinLength = exact.Length;
                    MaxLength = exact.Length;
                    break;
                case NotEmptyValidator:
                    MinLength = Math.Max(MinLength ?? 0, 1);
                    break;
                case RegexValidator regex:
                    _patterns.Add(regex.Regex.ToString());
                    break;
                case AllowedValuesValidator<TPrimitive> allowed:
                    AllowedValues = [.. allowed.AllowedValues.Select(static value => Text(value)).Order(StringComparer.Ordinal)];
                    break;
                default:
                    break;
            }
        }
    }

    private void SetMinimum(string value, bool exclusive)
    {
        Minimum = value;
        MinimumIsExclusive = exclusive;
    }

    private void SetMaximum(string value, bool exclusive)
    {
        Maximum = value;
        MaximumIsExclusive = exclusive;
    }

    private static string Text<T>(T value) where T : notnull =>
        value is IFormattable formattable ? formattable.ToString(null, CultureInfo.InvariantCulture) : value.ToString() ?? string.Empty;

    private readonly List<string> _patterns = [];
}

/// <summary>
///     The description of one strong type, which knows the strong type and its value as generic arguments.
/// </summary>
/// <typeparam name="TStrongType">The strong type.</typeparam>
/// <typeparam name="TPrimitive">Its value.</typeparam>
[PublicAPI]
public sealed class StrongTypeDescription<TStrongType, TPrimitive> : StrongTypeDescription
    where TStrongType : class, IStrongType<TStrongType, TPrimitive>
    where TPrimitive : notnull
{
    internal StrongTypeDescription(string? format) : base(format) { }

    /// <inheritdoc />
    public override Type Type => typeof(TStrongType);

    /// <inheritdoc />
    public override Type PrimitiveType => typeof(TPrimitive);

    /// <inheritdoc />
    public override TResult Accept<TResult>(IStrongTypeVisitor<TResult> visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        return visitor.Visit(this);
    }
}

/// <summary>
///     Does something with a strong type known by its generic arguments, such as building a converter for it.
/// </summary>
/// <typeparam name="TResult">What the visitor returns.</typeparam>
[PublicAPI]
public interface IStrongTypeVisitor<out TResult>
{
    /// <summary>
    ///     Visits one strong type.
    /// </summary>
    /// <param name="description">Its description.</param>
    /// <typeparam name="TStrongType">The strong type.</typeparam>
    /// <typeparam name="TPrimitive">Its value.</typeparam>
    /// <returns>The result.</returns>
    TResult Visit<TStrongType, TPrimitive>(StrongTypeDescription<TStrongType, TPrimitive> description)
        where TStrongType : class, IStrongType<TStrongType, TPrimitive>
        where TPrimitive : notnull;
}

/// <summary>
///     The format a strong type's value is written in, as OpenAPI and JSON Schema name it (<c>email</c>, <c>uri</c>, <c>ipv4</c>, …), on the
///     strong type or on its rules.
/// </summary>
/// <param name="format">The format.</param>
[PublicAPI]
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class StrongTypeFormatAttribute(string format) : Attribute
{
    /// <summary>
    ///     The format.
    /// </summary>
    public string Format { get; } = format;
}
