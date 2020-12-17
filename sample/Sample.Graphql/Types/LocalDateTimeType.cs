using NodaTime;
using NodaTime.Text;
using Sample.Graphql.Extensions;
using Sample.Graphql.Helpers;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Sample.Graphql.Types
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