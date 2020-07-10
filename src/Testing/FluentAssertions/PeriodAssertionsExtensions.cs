using System;
using NodaTime;
using Rocket.Surgery.LaunchPad.Testing.FluentAssertions.NodaTime;

// ReSharper disable once CheckNamespace
namespace FluentAssertions
{
    public static class PeriodAssertionsExtensions
    {
        public static PeriodAssertions Should(this Period value)
        {
            if(value == null) throw new ArgumentNullException(nameof(value));
            return new PeriodAssertions(value);
        }
    }
}