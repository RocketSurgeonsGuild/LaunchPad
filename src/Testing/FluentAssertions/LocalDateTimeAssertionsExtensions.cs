using System;
using NodaTime;
using Rocket.Surgery.LaunchPad.Testing.FluentAssertions.NodaTime;

// ReSharper disable once CheckNamespace
namespace FluentAssertions;

/// <summary>
///     <see cref="LocalDateTime" /> fluent assertion extensions
/// </summary>
public static class LocalDateTimeAssertionsExtensions
{
    /// <summary>
    ///     Match nullable <see cref="LocalDateTime" />
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static LocalDateTimeAssertions Should(this LocalDateTime? value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        return new LocalDateTimeAssertions(value);
    }

    /// <summary>
    ///     Match <see cref="LocalDateTime" />
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static LocalDateTimeAssertions Should(this LocalDateTime value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        return new LocalDateTimeAssertions(value);
    }
}
