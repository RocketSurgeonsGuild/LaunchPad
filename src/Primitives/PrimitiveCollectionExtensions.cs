namespace Rocket.Surgery.LaunchPad.Primitives;

/// <summary>
///     Creates domain primitives from collections of values.
/// </summary>
[PublicAPI]
public static class PrimitiveCollectionExtensions
{
    /// <summary>
    ///     Creates a domain primitive from every value, or throws for the first value its rules reject, naming its index.
    /// </summary>
    /// <param name="values">The values.</param>
    /// <typeparam name="TPrimitive">The domain primitive.</typeparam>
    /// <typeparam name="TValue">Its value.</typeparam>
    /// <returns>The domain primitives.</returns>
    public static List<TPrimitive> ToDomainPrimitiveList<TPrimitive, TValue>(this IEnumerable<TValue> values)
        where TPrimitive : IDomainPrimitive<TPrimitive, TValue>
        where TValue : notnull
    {
        ArgumentNullException.ThrowIfNull(values);
        var list = new List<TPrimitive>(values.TryGetNonEnumeratedCount(out var count) ? count : 4);
        var index = 0;
        foreach (var value in values)
        {
            list.Add(Create<TPrimitive, TValue>(value, index++, nameof(values)));
        }

        return list;
    }

    /// <summary>
    ///     Creates a domain primitive from every value, or throws for the first value its rules reject, naming its index.
    /// </summary>
    /// <param name="values">The values.</param>
    /// <typeparam name="TPrimitive">The domain primitive.</typeparam>
    /// <typeparam name="TValue">Its value.</typeparam>
    /// <returns>The domain primitives.</returns>
    public static TPrimitive[] ToDomainPrimitiveArray<TPrimitive, TValue>(this IEnumerable<TValue> values)
        where TPrimitive : IDomainPrimitive<TPrimitive, TValue>
        where TValue : notnull => [.. ToDomainPrimitiveList<TPrimitive, TValue>(values)];

    /// <summary>
    ///     Creates a domain primitive from every value, or throws for the first value its rules reject, naming its index.
    /// </summary>
    /// <param name="values">The values.</param>
    /// <typeparam name="TPrimitive">The domain primitive.</typeparam>
    /// <typeparam name="TValue">Its value.</typeparam>
    /// <returns>The domain primitives.</returns>
    public static TPrimitive[] ToDomainPrimitiveArray<TPrimitive, TValue>(this ReadOnlySpan<TValue> values)
        where TPrimitive : IDomainPrimitive<TPrimitive, TValue>
        where TValue : notnull
    {
        var array = new TPrimitive[values.Length];
        for (var i = 0; i < values.Length; i++)
        {
            array[i] = Create<TPrimitive, TValue>(values[i], i, nameof(values));
        }

        return array;
    }

    private static TPrimitive Create<TPrimitive, TValue>(TValue value, int index, string paramName)
        where TPrimitive : IDomainPrimitive<TPrimitive, TValue>
        where TValue : notnull =>
        TPrimitive.TryCreate(value, out var result, out var error)
            ? result
            : throw new ArgumentException($"Element at index {index} failed validation: [{error.Code}] {error.Message}", paramName, new DomainPrimitiveValidationException(error));
}
