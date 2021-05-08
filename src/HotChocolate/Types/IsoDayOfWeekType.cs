using NodaTime;
using Rocket.Surgery.LaunchPad.HotChocolate.Helpers;
using System.Diagnostics.CodeAnalysis;

namespace Rocket.Surgery.LaunchPad.HotChocolate.Types
{
    /// <summary>
    /// Represents an <see cref="IsoDayOfWeek" /> in Hot Chocolate
    /// </summary>
    public class IsoDayOfWeekType : IntToStructBaseType<IsoDayOfWeek>
    {
        /// <summary>
        /// The constructor
        /// </summary>
        public IsoDayOfWeekType() : base("IsoDayOfWeek")
        {
            Description =
                "Equates the days of the week with their numerical value according to ISO-8601.\n" +
                "Monday = 1, Tuesday = 2, Wednesday = 3, Thursday = 4, Friday = 5, Saturday = 6, Sunday = 7.";
        }

        /// <inheritdoc />
        protected override bool TrySerialize(IsoDayOfWeek baseValue, [NotNullWhen(true)] out int? output)
        {
            if (baseValue == IsoDayOfWeek.None)
            {
                output = null;
                return false;
            }

            output = (int)baseValue;
            return true;
        }

        /// <inheritdoc />
        protected override bool TryDeserialize(int val, [NotNullWhen(true)] out IsoDayOfWeek? output)
        {
            if (val < 1 || val > 7)
            {
                output = null;
                return false;
            }

            output = (IsoDayOfWeek)val;
            return true;
        }
    }
}