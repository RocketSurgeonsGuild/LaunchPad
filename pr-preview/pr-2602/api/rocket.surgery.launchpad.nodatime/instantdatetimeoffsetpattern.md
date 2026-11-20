# InstantDateTimeOffsetPattern

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.NodaTime](/LaunchPad/pr-preview/pr-2602/api/rocket.surgery.launchpad.nodatime.md)
- Version: `2.0.1-pr.1260`
- Namespace: `Rocket.Surgery.LaunchPad.NodaTime`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-2602-merge.Sha.022b9cee91d8927e916725e8a1c1717558619950/src/NodaTime/InstantDateTimeOffsetPattern.cs)
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

- [InstantDateTimeOffsetPattern](/LaunchPad/pr-preview/pr-2602/api/rocket.surgery.launchpad.nodatime/instantdatetimeoffsetpattern/constructors.md#constructor)

## Methods

- [AppendFormat(Instant, StringBuilder)](/LaunchPad/pr-preview/pr-2602/api/rocket.surgery.launchpad.nodatime/instantdatetimeoffsetpattern/methods.md#appendformat-instant-stringbuilder) : `StringBuilder` -- Formats the given value as text according to the rules of this pattern, appending to the given `Text.StringBuilder`.
- [Format(Instant)](/LaunchPad/pr-preview/pr-2602/api/rocket.surgery.launchpad.nodatime/instantdatetimeoffsetpattern/methods.md#format-instant) : `string` -- Formats the given value as text according to the rules of this pattern.
- [Parse(string)](/LaunchPad/pr-preview/pr-2602/api/rocket.surgery.launchpad.nodatime/instantdatetimeoffsetpattern/methods.md#parse-string) : `ParseResult<Instant>` -- Parses the given text value according to the rules of this pattern.
