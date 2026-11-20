namespace Rocket.Surgery.LaunchPad.Foundation;

/// <summary>
///     A mutable copy of an immutable value, such as the view model <see cref="MutableAttribute" /> generates for a request, that can make the
///     value it stands for.
/// </summary>
[PublicAPI]
public interface IMutable
{
    /// <summary>
    ///     Makes the immutable value from the current values of the mutable copy.
    /// </summary>
    /// <returns>The immutable value, boxed when it is a struct.</returns>
    object ToImmutable();
}

/// <summary>
///     A mutable copy of an immutable <typeparamref name="T" />, such as the view model <see cref="MutableAttribute" /> generates for a request: a
///     form binds to it a property at a time, and it makes the <typeparamref name="T" /> to send.
/// </summary>
/// <typeparam name="T">The immutable type.</typeparam>
[PublicAPI]
public interface IMutable<out T> : IMutable
{
    /// <summary>
    ///     Makes the <typeparamref name="T" /> from the current values of the mutable copy.
    /// </summary>
    /// <returns>The immutable value.</returns>
    new T ToImmutable();
}
