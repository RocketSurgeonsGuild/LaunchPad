using NodaTime;
using Rocket.Surgery.LaunchPad.Testing.FluentAssertions.NodaTime;

// ReSharper disable once CheckNamespace
namespace FluentAssertions;

/// <summary>
///     <see cref="LocalDate" /> fluent assertion extensions
/// </summary>
public static class LocalDateAssertionsExtensions
{
    /// <summary>
    ///     Match nullable <see cref="LocalDate" />
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static LocalDateAssertions Should(this LocalDate? value)
    {
        return new LocalDateAssertions(value);
    }

    /// <summary>
    ///     Match <see cref="LocalDate" />
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static LocalDateAssertions Should(this LocalDate value)
    {
        return new LocalDateAssertions(value);
    }
}
