using System;
using NodaTime;
using Rocket.Surgery.LaunchPad.Testing.FluentAssertions.NodaTime;

// ReSharper disable once CheckNamespace
namespace FluentAssertions;

/// <summary>
///     <see cref="Duration" /> fluent assertion extensions
/// </summary>
public static class DurationAssertionsExtensions
{
    /// <summary>
    ///     Match nullable <see cref="Duration" />
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static DurationAssertions Should(this Duration? value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        return new DurationAssertions(value);
    }

    /// <summary>
    ///     Match <see cref="Duration" />
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static DurationAssertions Should(this Duration value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        return new DurationAssertions(value);
    }
}
