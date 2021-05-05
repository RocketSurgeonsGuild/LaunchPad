using NodaTime;
using NodaTime.Text;
using Rocket.Surgery.LaunchPad.HotChocolate.Extensions;
using Rocket.Surgery.LaunchPad.HotChocolate.Helpers;
using System.Diagnostics.CodeAnalysis;

namespace Rocket.Surgery.LaunchPad.HotChocolate.Types
{
    /// <summary>
    /// Represents a <see cref="Period"/> in Hot Chocolate
    /// </summary>
    public class PeriodType : StringToClassBaseType<Period>
    {
        /// <summary>
        /// The constructor
        /// </summary>
        public PeriodType() : base("Period")
        {
            Description =
                "Represents a period of time expressed in human chronological " +
                    "terms: hours, days, weeks, months and so on.";
        }

        /// <inheritdoc />
        protected override string Serialize(Period baseValue)
            => PeriodPattern.Roundtrip
                .Format(baseValue);

        /// <inheritdoc />
        protected override bool TryDeserialize(string str, [NotNullWhen(true)] out Period? output)
            => PeriodPattern.Roundtrip
                .TryParse(str, out output);
    }
}