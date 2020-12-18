using NodaTime;
using NodaTime.Text;
using Rocket.Surgery.LaunchPad.HotChocolate.Extensions;
using Rocket.Surgery.LaunchPad.HotChocolate.Helpers;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Rocket.Surgery.LaunchPad.HotChocolate.Types
{
    public class OffsetType : StringToStructBaseType<Offset>
    {
        public OffsetType() : base("Offset")
        {
            Description =
                "An offset from UTC in seconds.\n" +
                "A positive value means that the local time is ahead of UTC (e.g. for Europe); " +
                    "a negative value means that the local time is behind UTC (e.g. for America).";
        }

        protected override string Serialize(Offset baseValue)
            => OffsetPattern.GeneralInvariantWithZ
                .WithCulture(CultureInfo.InvariantCulture)
                .Format(baseValue);

        protected override bool TryDeserialize(string str, [NotNullWhen(true)] out Offset? output)
            => OffsetPattern.GeneralInvariantWithZ
                .WithCulture(CultureInfo.InvariantCulture)
                .TryParse(str, out output);
    }
}