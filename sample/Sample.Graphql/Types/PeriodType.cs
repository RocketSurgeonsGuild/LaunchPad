using NodaTime;
using NodaTime.Text;
using Sample.Graphql.Extensions;
using Sample.Graphql.Helpers;
using System.Diagnostics.CodeAnalysis;

namespace Sample.Graphql.Types
{
    public class PeriodType : StringToClassBaseType<Period>
    {
        public PeriodType() : base("Period")
        {
            Description =
                "Represents a period of time expressed in human chronological " +
                    "terms: hours, days, weeks, months and so on.";
        }

        protected override string Serialize(Period baseValue)
            => PeriodPattern.Roundtrip
                .Format(baseValue);

        protected override bool TryDeserialize(string str, [NotNullWhen(true)] out Period? output)
            => PeriodPattern.Roundtrip
                .TryParse(str, out output);
    }
}