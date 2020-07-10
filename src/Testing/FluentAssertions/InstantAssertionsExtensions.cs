using System;
using NodaTime;
using Rocket.Surgery.LaunchPad.Testing.FluentAssertions.NodaTime;

// ReSharper disable once CheckNamespace
namespace FluentAssertions
{
    public static class InstantAssertionsExtensions
    {
        public static InstantAssertions Should(this Instant? value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            return new InstantAssertions(value);
        }

        public static InstantAssertions Should(this Instant value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            return new InstantAssertions(value);
        }
    }
}
