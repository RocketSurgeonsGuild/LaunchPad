using System;
using NodaTime;
using Rocket.Surgery.LaunchPad.Testing.FluentAssertions.NodaTime;

// ReSharper disable once CheckNamespace
namespace FluentAssertions
{
    public static class DurationAssertionsExtensions
    {
        public static DurationAssertions Should(this Duration? value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            return new DurationAssertions(value);
        }

        public static DurationAssertions Should(this Duration value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            return new DurationAssertions(value);
        }
    }
}
