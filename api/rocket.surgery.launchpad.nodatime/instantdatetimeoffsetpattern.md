# InstantDateTimeOffsetPattern

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.NodaTime](/LaunchPad/api/rocket.surgery.launchpad.nodatime.md)
- Version: `2.0.1-beta.425`
- Namespace: `Rocket.Surgery.LaunchPad.NodaTime`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.d4b5f5c52aac2c2cd0e8fd565d77879e394d9792/src/NodaTime/InstantDateTimeOffsetPattern.cs)
- Implements: `IPattern<Instant>`

A pattern used to create an instant from a serialized DateTimeOffset value

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.NodaTime;

public class InstantDateTimeOffsetPattern
    : NodaTime.Text.IPattern<NodaTime.Instant>
{
    // ...
}
```

## Constructors

- [InstantDateTimeOffsetPattern](/LaunchPad/api/rocket.surgery.launchpad.nodatime/instantdatetimeoffsetpattern/constructors.md#constructor)

## Methods

- [AppendFormat(Instant, StringBuilder)](/LaunchPad/api/rocket.surgery.launchpad.nodatime/instantdatetimeoffsetpattern/methods.md#appendformat-instant-stringbuilder) : `StringBuilder` -- Formats the given value as text according to the rules of this pattern, appending to the given `Text.StringBuilder`.
- [Format(Instant)](/LaunchPad/api/rocket.surgery.launchpad.nodatime/instantdatetimeoffsetpattern/methods.md#format-instant) : `string` -- Formats the given value as text according to the rules of this pattern.
- [Parse(string)](/LaunchPad/api/rocket.surgery.launchpad.nodatime/instantdatetimeoffsetpattern/methods.md#parse-string) : `ParseResult<Instant>` -- Parses the given text value according to the rules of this pattern.
