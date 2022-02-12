using System.Text;
using NodaTime;
using NodaTime.Extensions;
using NodaTime.Text;

namespace Rocket.Surgery.LaunchPad.Foundation;

internal class NewtonsoftJsonDateTimeOffsetPattern : IPattern<Instant>
{
    public ParseResult<Instant> Parse(string text)
    {
        return DateTimeOffset.TryParse(text, out var value)
            ? ParseResult<Instant>.ForValue(value.ToInstant())
            : ParseResult<Instant>.ForException(() => new FormatException("Could not parse DateTimeOffset"));
    }

    public string Format(Instant value)
    {
        return InstantPattern.ExtendedIso.Format(value);
    }

    public StringBuilder AppendFormat(Instant value, StringBuilder builder)
    {
        return InstantPattern.ExtendedIso.AppendFormat(value, builder);
    }
}
