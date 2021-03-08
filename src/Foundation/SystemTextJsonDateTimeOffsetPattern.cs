using NodaTime;
using NodaTime.Extensions;
using NodaTime.Text;
using System;
using System.Text;

namespace Rocket.Surgery.LaunchPad.AspNetCore
{
    class SystemTextJsonDateTimeOffsetPattern : IPattern<Instant>
    {
        public ParseResult<Instant> Parse(string text) => DateTimeOffset.TryParse(text, out var value)
            ? ParseResult<Instant>.ForValue(value.ToInstant())
            : ParseResult<Instant>.ForException(() => new FormatException("Could not parse DateTimeOffset"));

        public string Format(Instant value) => InstantPattern.General.Format(value);
        public StringBuilder AppendFormat(Instant value, StringBuilder builder) => InstantPattern.General.AppendFormat(value, builder);
    }
}