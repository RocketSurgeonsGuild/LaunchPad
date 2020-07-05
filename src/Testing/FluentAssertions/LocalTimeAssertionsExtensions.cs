using System;
using NodaTime;
using Rocket.Surgery.LaunchPad.Testing.FluentAssertions.NodaTime;

// ReSharper disable once CheckNamespace
namespace FluentAssertions
{
    public static class LocalTimeAssertionsExtensions
    {
        public static LocalTimeAssertions Should(this LocalTime? value)
        {
            if(value == null) throw new ArgumentNullException(nameof(value));
            return new LocalTimeAssertions(value);
        }

        public static LocalTimeAssertions Should(this LocalTime value)
        {
            return new LocalTimeAssertions(value);
        }
    }
}