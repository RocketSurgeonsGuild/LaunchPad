namespace Rocket.Surgery.LaunchPad.Foundation;
#pragma warning disable CA2225

/// <summary>
///     The Assigned type is used to differentiate between not set and set input values.
/// </summary>
[PublicAPI]
public class Assigned<T> : IEquatable<Assigned<T>>
{
    /// <summary>
    ///     Operator call through to Equals
    /// </summary>
    /// <param name="left">The left parameter</param>
    /// <param name="right">The right parameter</param>
    /// <returns>
    ///     <c>true</c> if both <see cref="Assigned{T}" /> values are equal.
    /// </returns>
    public static bool operator ==(Assigned<T> left, Assigned<T> right)
    {
        return left.Equals(right);
    }

    /// <summary>
    ///     Operator call through to Equals
    /// </summary>
    /// <param name="left">The left parameter</param>
    /// <param name="right">The right parameter</param>
    /// <returns>
    ///     <c>true</c> if both <see cref="Assigned{T}" /> values are not equal.
    /// </returns>
    public static bool operator !=(Assigned<T> left, Assigned<T> right)
    {
        return !left.Equals(right);
    }

    /// <summary>
    ///     Implicitly creates a new <see cref="Assigned{T}" /> from
    ///     the given value.
    /// </summary>
    /// <param name="value">The value.</param>
    public static implicit operator Assigned<T>(T value)
    {
        return new(value);
    }

    /// <summary>
    ///     Implicitly gets the Assigned value.
    /// </summary>
    public static implicit operator T?(Assigned<T> assigned)
    {
        return assigned.Value;
    }

    /// <summary>
    ///     Creates an empty Assigned that provides a default value.
    /// </summary>
    /// <param name="defaultValue">The default value.</param>
#pragma warning disable CA1000
    public static Assigned<T> Empty(T? defaultValue = default)
#pragma warning restore CA1000
    {
        return new(defaultValue, false);
    }

    private readonly bool _hasValue;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Assigned{T}" /> struct.
    /// </summary>
    /// <param name="value">The actual value.</param>
    public Assigned(T? value)
    {
        Value = value;
        _hasValue = true;
    }

    private Assigned(T? value, bool hasValue)
    {
        Value = value;
        _hasValue = hasValue;
    }

    /// <summary>
    ///     The name value.
    /// </summary>
    [MaybeNull]
    public T? Value { get; }

    /// <summary>
    ///     <c>true</c> if the Assigned was explicitly set.
    /// </summary>
    /// <returns></returns>
    public bool HasBeenSet()
    {
        return _hasValue;
    }

    /// <summary>
    ///     Provides the name string.
    /// </summary>
    /// <returns>The name string value</returns>
    public override string ToString()
    {
        return _hasValue ? Value?.ToString() ?? "null" : "unspecified";
    }

    /// <summary>
    ///     Compares this <see cref="Assigned{T}" /> value to another value.
    /// </summary>
    /// <param name="obj">
    ///     The second <see cref="Assigned{T}" /> for comparison.
    /// </param>
    /// <returns>
    ///     <c>true</c> if both <see cref="Assigned{T}" /> values are equal.
    /// </returns>
    public override bool Equals(object? obj)
    {
        if (obj is null) return !_hasValue;

        return obj is Assigned<T> n && Equals(n);
    }

    /// <summary>
    ///     Serves as a hash function for a <see cref="Assigned{T}" /> object.
    /// </summary>
    /// <returns>
    ///     A hash code for this instance that is suitable for use in hashing
    ///     algorithms and data structures such as a hash table.
    /// </returns>
    public override int GetHashCode()
    {
        return _hasValue ? Value?.GetHashCode() ?? 0 : 0;
    }

    /// <summary>
    ///     Compares this <see cref="Assigned{T}" /> value to another value.
    /// </summary>
    /// <param name="other">
    ///     The second <see cref="Assigned{T}" /> for comparison.
    /// </param>
    /// <returns>
    ///     <c>true</c> if both <see cref="Assigned{T}" /> values are equal.
    /// </returns>
    public bool Equals(Assigned<T>? other)
    {
        if (other is null) return !_hasValue;

        if (!_hasValue && !other._hasValue) return true;

        if (_hasValue != other._hasValue) return false;

        return Equals(Value, other.Value);
    }
}
