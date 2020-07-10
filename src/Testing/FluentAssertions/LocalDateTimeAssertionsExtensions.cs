using System;
using NodaTime;
using Rocket.Surgery.LaunchPad.Testing.FluentAssertions.NodaTime;

// ReSharper disable once CheckNamespace
namespace FluentAssertions
{
    public static class LocalDateTimeAssertionsExtensions
    {
        public static LocalDateTimeAssertions Should(this LocalDateTime? value)
        {
            if(value == null) throw new ArgumentNullException(nameof(value));
            return new LocalDateTimeAssertions(value);
        }

        public static LocalDateTimeAssertions Should(this LocalDateTime value)
        {
            if(value == null) throw new ArgumentNullException(nameof(value));
            return new LocalDateTimeAssertions(value);
        }
    }
}