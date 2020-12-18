using NodaTime;
using NodaTime.Text;
using Rocket.Surgery.LaunchPad.HotChocolate.Extensions;
using Rocket.Surgery.LaunchPad.HotChocolate.Helpers;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Rocket.Surgery.LaunchPad.HotChocolate.Types
{
    public class DurationType : StringToStructBaseType<Duration>
    {
        public DurationType() : base("Duration")
        {
            Description = "Represents a fixed (and calendar-independent) length of time.";
        }

        protected override string Serialize(Duration baseValue)
            => DurationPattern.Roundtrip
                .WithCulture(CultureInfo.InvariantCulture)
                .Format(baseValue);

        protected override bool TryDeserialize(string str, [NotNullWhen(true)] out Duration? output)
            => DurationPattern.Roundtrip
                .WithCulture(CultureInfo.InvariantCulture)
                .TryParse(str, out output);
    }
}