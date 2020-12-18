using NodaTime;
using NodaTime.Text;
using Rocket.Surgery.LaunchPad.HotChocolate.Extensions;
using Rocket.Surgery.LaunchPad.HotChocolate.Helpers;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Rocket.Surgery.LaunchPad.HotChocolate.Types
{
    public class InstantType : StringToStructBaseType<Instant>
    {
        public InstantType() : base("Instant")
        {
            Description = "Represents an instant on the global timeline, with nanosecond resolution.";
        }

        protected override string Serialize(Instant val)
            => InstantPattern.ExtendedIso
                .WithCulture(CultureInfo.InvariantCulture)
                .Format(val);

        protected override bool TryDeserialize(string str, [NotNullWhen(true)] out Instant? output)
            => InstantPattern.ExtendedIso
                .WithCulture(CultureInfo.InvariantCulture)
                .TryParse(str, out output);
    }
}