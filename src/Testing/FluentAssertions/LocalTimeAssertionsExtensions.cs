using NodaTime;
using Rocket.Surgery.LaunchPad.Testing.FluentAssertions.NodaTime;
using System;

// ReSharper disable once CheckNamespace
namespace FluentAssertions
{
    /// <summary>
    /// <see cref="LocalTime"/> fluent assertion extensions
    /// </summary>
    public static class LocalTimeAssertionsExtensions
    {
        /// <summary>
        /// Match nullable <see cref="LocalTime"/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static LocalTimeAssertions Should(this LocalTime? value)
        {
            if(value == null) throw new ArgumentNullException(nameof(value));
            return new LocalTimeAssertions(value);
        }

        /// <summary>
        /// Match <see cref="LocalTime"/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static LocalTimeAssertions Should(this LocalTime value)
        {
            return new LocalTimeAssertions(value);
        }
    }
}