using System;
using NodaTime;
using Rocket.Surgery.LaunchPad.Testing.FluentAssertions.NodaTime;

// ReSharper disable once CheckNamespace
namespace FluentAssertions
{
    public static class LocalDateAssertionsExtensions
    {
        public static LocalDateAssertions Should(this LocalDate? value)
        {
            if(value == null) throw new ArgumentNullException(nameof(value));
            return new LocalDateAssertions(value);
        }

        public static LocalDateAssertions Should(this LocalDate value)
        {
            return new LocalDateAssertions(value);
        }
    }
}