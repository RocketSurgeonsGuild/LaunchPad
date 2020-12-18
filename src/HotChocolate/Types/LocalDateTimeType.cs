using NodaTime;
using NodaTime.Text;
using Rocket.Surgery.LaunchPad.HotChocolate.Extensions;
using Rocket.Surgery.LaunchPad.HotChocolate.Helpers;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Rocket.Surgery.LaunchPad.HotChocolate.Types
{
    public class LocalDateTimeType : StringToStructBaseType<LocalDateTime>
    {
        public LocalDateTimeType() : base("LocalDateTime")
        {
            Description = "A date and time in a particular calendar system.";
        }

        protected override string Serialize(LocalDateTime baseValue)
            => LocalDateTimePattern.ExtendedIso
                .WithCulture(CultureInfo.InvariantCulture)
                .Format(baseValue);

        protected override bool TryDeserialize(string str, [NotNullWhen(true)] out LocalDateTime? output)
            => LocalDateTimePattern.ExtendedIso
                .WithCulture(CultureInfo.InvariantCulture)
                .TryParse(str, out output);
    }
}