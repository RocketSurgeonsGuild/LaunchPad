using System.Globalization;
using System.Text;
using NodaTime;
using NodaTime.Extensions;
using NodaTime.Text;

namespace Rocket.Surgery.LaunchPad.Primitives;

/// <summary>
///     A pattern used to create an instant from a serialized DateTimeOffset value
/// </summary>
public class InstantDateTimeOffsetPattern : IPattern<Instant>
{
    /// <inheritdoc />
    public ParseResult<Instant> Parse(string text) => DateTimeOffset.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var value)
        ? ParseResult<Instant>.ForValue(value.ToInstant())
        : ParseResult<Instant>.ForException(() => new FormatException("Could not parse DateTimeOffset"));

    /// <inheritdoc />
    public string Format(Instant value) => InstantPattern.General.Format(value);

    /// <inheritdoc />
    public StringBuilder AppendFormat(Instant value, StringBuilder builder) => InstantPattern.General.AppendFormat(value, builder);
}
