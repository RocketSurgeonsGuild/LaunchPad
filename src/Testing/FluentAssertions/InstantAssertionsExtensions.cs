using NodaTime;
using Rocket.Surgery.LaunchPad.Testing.FluentAssertions.NodaTime;

// ReSharper disable once CheckNamespace
namespace FluentAssertions;

/// <summary>
///     <see cref="Instant" /> fluent assertion extenisons
/// </summary>
public static class InstantAssertionsExtensions
{
    /// <summary>
    ///     Match nullable <see cref="Instant" />
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static InstantAssertions Should(this Instant? value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        return new InstantAssertions(value);
    }

    /// <summary>
    ///     Match <see cref="Instant" />
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static InstantAssertions Should(this Instant value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        return new InstantAssertions(value);
    }
}
