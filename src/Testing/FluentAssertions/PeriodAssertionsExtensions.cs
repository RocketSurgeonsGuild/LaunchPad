using System;
using NodaTime;
using Rocket.Surgery.LaunchPad.Testing.FluentAssertions.NodaTime;

// ReSharper disable once CheckNamespace
namespace FluentAssertions
{
    /// <summary>
    /// <see cref="Period"/> Assertion Extenisons
    /// </summary>
    public static class PeriodAssertionsExtensions
    {
        /// <summary>
        /// Compares the given period
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static PeriodAssertions Should(this Period value)
        {
            if(value == null) throw new ArgumentNullException(nameof(value));
            return new PeriodAssertions(value);
        }
    }
}