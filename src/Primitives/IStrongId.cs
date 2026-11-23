namespace Rocket.Surgery.LaunchPad.Primitives;

/// <summary>
///     A strong ID: a domain primitive that identifies an entity.
/// </summary>
/// <typeparam name="TSelf">The strong ID.</typeparam>
/// <typeparam name="TValue">The value it holds: a <see cref="Guid" />, <see cref="int" />, <see cref="long" /> or <see cref="string" />.</typeparam>
[PublicAPI]
public interface IStrongId<TSelf, TValue> : IDomainPrimitive<TSelf, TValue>
    where TSelf : IStrongId<TSelf, TValue>
    where TValue : notnull
{
    /// <summary>
    ///     Creates a new ID. Only an ID of a <see cref="Guid" /> can be created; any other throws <see cref="NotSupportedException" />.
    /// </summary>
    /// <returns>The new ID.</returns>
    abstract static TSelf Create();

    /// <summary>
    ///     The empty ID.
    /// </summary>
    abstract static TSelf Empty { get; }
}
