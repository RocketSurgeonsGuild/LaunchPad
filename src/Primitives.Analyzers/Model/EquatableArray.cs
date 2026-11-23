using System.Collections;
using System.Collections.Immutable;

namespace Rocket.Surgery.LaunchPad.Primitives.Analyzers.Model;

/// <summary>
///     An immutable array compared by its items, so a model that holds one stays cacheable in an incremental generator.
/// </summary>
internal readonly struct EquatableArray<T>(ImmutableArray<T> items) : IEquatable<EquatableArray<T>>, IEnumerable<T>
    where T : IEquatable<T>
{
    public static EquatableArray<T> Empty { get; } = new([]);

    public ImmutableArray<T> Items => items.IsDefault ? [] : items;
    public int Length => Items.Length;
    public T this[int index] => Items[index];

    public bool Equals(EquatableArray<T> other) => Items.AsSpan().SequenceEqual(other.Items.AsSpan());
    public override bool Equals(object? obj) => obj is EquatableArray<T> other && Equals(other);

    public override int GetHashCode()
    {
        var hash = 17;
        foreach (var item in Items)
        {
            hash = unchecked(( hash * 31 ) + ( item?.GetHashCode() ?? 0 ));
        }

        return hash;
    }

    public IEnumerator<T> GetEnumerator() => ( (IEnumerable<T>)Items ).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public static implicit operator EquatableArray<T>(ImmutableArray<T> items) => new(items);
}

internal static class EquatableArray
{
    public static EquatableArray<T> ToEquatableArray<T>(this IEnumerable<T> items) where T : IEquatable<T> => new([.. items]);
}
