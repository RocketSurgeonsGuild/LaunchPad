namespace Rocket.Surgery.LaunchPad.Foundation.Extensions;

/// <summary>
///     Useful extensions for enumerations.
/// </summary>
public static class UsefulEnumerableExtensions
{
    /// <summary>
    ///     Allows you to enumerate over an enumerator you have without the need for it to be an <see cref="IEnumerable{T}" />.
    /// </summary>
    /// <param name="enumerator"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IEnumerator<T> GetEnumerator<T>(this IEnumerator<T> enumerator)
    {
        return enumerator;
    }

    /// <summary>
    ///     Allows you to enumerate over an enumerator you have without the need for it to be an <see cref="IAsyncEnumerator{T}" />.
    /// </summary>
    /// <param name="enumerator"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IAsyncEnumerator<T> GetAsyncEnumerator<T>(this IAsyncEnumerator<T> enumerator)
    {
        return enumerator;
    }
}
