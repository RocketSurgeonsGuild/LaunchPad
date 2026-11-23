namespace Rocket.Surgery.LaunchPad.Primitives;

/// <summary>
///     A domain primitive: a type the Primitives generator completes, which only holds values its rules accept.
/// </summary>
/// <typeparam name="TSelf">The domain primitive.</typeparam>
[PublicAPI]
public interface IDomainPrimitive<TSelf>
    where TSelf : IDomainPrimitive<TSelf>
{
    /// <summary>
    ///     The name of the domain primitive, used in errors and diagnostics.
    /// </summary>
    abstract static string PrimitiveName { get; }

    /// <summary>
    ///     Whether this is the default value of a domain primitive declared as a struct, which holds no value. A domain primitive declared as a
    ///     class is never default: a missing value is <see langword="null" />.
    /// </summary>
    bool IsDefault => false;
}

/// <summary>
///     A domain primitive that holds one value.
/// </summary>
/// <typeparam name="TSelf">The domain primitive.</typeparam>
/// <typeparam name="TValue">The value it holds.</typeparam>
[PublicAPI]
public interface IDomainPrimitive<TSelf, TValue> : IDomainPrimitive<TSelf>
    where TSelf : IDomainPrimitive<TSelf, TValue>
    where TValue : notnull
{
    /// <summary>
    ///     The value.
    /// </summary>
    TValue Value { get; }

    /// <summary>
    ///     Creates the domain primitive from a value, or throws when its rules reject the value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The domain primitive.</returns>
    abstract static TSelf Create(TValue value);

    /// <summary>
    ///     Creates the domain primitive from a value, or says why its rules reject the value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="result">The domain primitive, when the value is accepted.</param>
    /// <param name="validationError">Why the value was rejected, when it was.</param>
    /// <returns>Whether the value was accepted.</returns>
    abstract static bool TryCreate(TValue value, [NotNullWhen(true)] out TSelf? result, out PrimitiveError validationError);
}

/// <summary>
///     A value object: a domain primitive that holds several values, validated together.
/// </summary>
/// <typeparam name="TSelf">The value object.</typeparam>
[PublicAPI]
public interface IValueObject<TSelf> : IDomainPrimitive<TSelf>
    where TSelf : IValueObject<TSelf>;
