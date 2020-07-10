using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using NodaTime;

namespace Rocket.Surgery.LaunchPad.EntityFramework
{
    [PublicAPI]
    public class NodaTimeValueGenerator : ValueGenerator<DateTimeOffset>
    {
        private readonly IClock _clock;
        public NodaTimeValueGenerator(IClock clock) => _clock = clock;
        public override DateTimeOffset Next(EntityEntry entry) => _clock.GetCurrentInstant().ToDateTimeOffset();

        public override bool GeneratesTemporaryValues { get; } = false;
    }
}