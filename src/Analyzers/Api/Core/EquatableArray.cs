using System.Collections;
using System.Collections.Immutable;

namespace Rocket.Surgery.LaunchPad.Api.Generators;

/// <summary>
///     An immutable array with value equality, so that incremental generator models compare by content and cache correctly.
/// </summary>
internal readonly struct EquatableArray<T>(ImmutableArray<T> array) : IEquatable<EquatableArray<T>>, IEnumerable<T>
    where T : IEquatable<T>
{
    public static readonly EquatableArray<T> Empty = new([]);

    private readonly ImmutableArray<T> _array = array;

    public ImmutableArray<T> AsImmutableArray() => _array.IsDefault ? [] : _array;

    public int Length => AsImmutableArray().Length;

    public bool IsEmpty => Length == 0;

    public T this[int index] => AsImmutableArray()[index];

    public bool Equals(EquatableArray<T> other) => AsImmutableArray().AsSpan().SequenceEqual(other.AsImmutableArray().AsSpan());

    public override bool Equals(object? obj) => obj is EquatableArray<T> other && Equals(other);

    public override int GetHashCode()
    {
        var hash = 17;
        foreach (var item in AsImmutableArray())
        {
            hash = unchecked(hash * 31 + ( item?.GetHashCode() ?? 0 ));
        }

        return hash;
    }

    public ImmutableArray<T>.Enumerator GetEnumerator() => AsImmutableArray().GetEnumerator();

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => ( (IEnumerable<T>)AsImmutableArray() ).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ( (IEnumerable)AsImmutableArray() ).GetEnumerator();

    public static bool operator ==(EquatableArray<T> left, EquatableArray<T> right) => left.Equals(right);

    public static bool operator !=(EquatableArray<T> left, EquatableArray<T> right) => !left.Equals(right);
}

internal static class EquatableArray
{
    public static EquatableArray<T> ToEquatableArray<T>(this IEnumerable<T> source)
        where T : IEquatable<T> => new([.. source]);
}
