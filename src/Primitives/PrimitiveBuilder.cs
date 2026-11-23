using System.ComponentModel;

namespace Rocket.Surgery.LaunchPad.Primitives.Advanced;

/// <summary>
///     Creates a domain primitive with rules of the caller's own on top of its rules.
/// </summary>
/// <typeparam name="TPrimitive">The domain primitive.</typeparam>
/// <typeparam name="TValue">Its value.</typeparam>
[PublicAPI]
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class PrimitiveBuilder<TPrimitive, TValue>
    where TPrimitive : IDomainPrimitive<TPrimitive, TValue>
    where TValue : notnull
{
    /// <summary>
    ///     Starts a builder.
    /// </summary>
    /// <returns>The builder.</returns>
    public static PrimitiveBuilder<TPrimitive, TValue> For() => new();

    private PrimitiveBuilder() { }

    /// <summary>
    ///     Sets the value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The builder.</returns>
    public PrimitiveBuilder<TPrimitive, TValue> WithValue(TValue value)
    {
        _value = value;
        return this;
    }

    /// <summary>
    ///     Adds a rule.
    /// </summary>
    /// <param name="predicate">Whether the value is accepted.</param>
    /// <param name="errorCode">The code when it is not.</param>
    /// <param name="errorMessage">The message when it is not.</param>
    /// <returns>The builder.</returns>
    public PrimitiveBuilder<TPrimitive, TValue> Must(Func<TValue, bool> predicate, string errorCode, string errorMessage)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        _rules.Add(value => predicate(value) ? PrimitiveError.None : new(errorCode, errorMessage));
        return this;
    }

    /// <summary>
    ///     Creates the domain primitive, or throws when a rule rejects the value.
    /// </summary>
    /// <returns>The domain primitive.</returns>
    public TPrimitive BuildOrThrow()
    {
        if (_value is null) throw new DomainPrimitiveValidationException(new("NULL_INPUT", "Value was not provided to PrimitiveBuilder."));
        foreach (var rule in _rules)
        {
            if (rule(_value) is { IsError: true } error) throw new DomainPrimitiveValidationException(error);
        }

        return TPrimitive.Create(_value);
    }

    /// <summary>
    ///     Creates the domain primitive, unless a rule rejects the value.
    /// </summary>
    /// <param name="result">The domain primitive.</param>
    /// <returns>Whether the value was accepted.</returns>
    public bool Build([NotNullWhen(true)] out TPrimitive? result)
    {
        result = default;
        if (_value is null) return false;
        foreach (var rule in _rules)
        {
            if (rule(_value).IsError) return false;
        }

        return TPrimitive.TryCreate(_value, out result, out _);
    }

    private readonly List<Func<TValue, PrimitiveError>> _rules = [];
    private TValue? _value;
}
