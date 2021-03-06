using NodaTime;
using NodaTime.Text;
using Rocket.Surgery.LaunchPad.HotChocolate.Extensions;
using Rocket.Surgery.LaunchPad.HotChocolate.Helpers;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Rocket.Surgery.LaunchPad.HotChocolate.Types
{
    /// <summary>
    /// Represents an <see cref="Instant"/> in Hot Chocolate
    /// </summary>
    public class InstantType : StringToStructBaseType<Instant>
    {
        /// <summary>
        /// The constrcutor
        /// </summary>
        public InstantType() : base("Instant")
        {
            Description = "Represents an instant on the global timeline, with nanosecond resolution.";
        }

        /// <inheritdoc />
        protected override string Serialize(Instant val)
            => InstantPattern.ExtendedIso
                .WithCulture(CultureInfo.InvariantCulture)
                .Format(val);

        /// <inheritdoc />
        protected override bool TryDeserialize(string str, [NotNullWhen(true)] out Instant? output)
            => InstantPattern.ExtendedIso
                .WithCulture(CultureInfo.InvariantCulture)
                .TryParse(str, out output);
    }
}