# InstantDateTimeOffsetPattern

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.NodaTime](/LaunchPad/pr-preview/pr-3330/api/rocket.surgery.launchpad.nodatime.md)
- Version: `2.0.1-pr.1282`
- Namespace: `Rocket.Surgery.LaunchPad.NodaTime`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3330-merge.Sha.8f1e0adb726291eaf19241f8ed2aef23d09d52f6/src/NodaTime/InstantDateTimeOffsetPattern.cs)
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

- [InstantDateTimeOffsetPattern](/LaunchPad/pr-preview/pr-3330/api/rocket.surgery.launchpad.nodatime/instantdatetimeoffsetpattern/constructors.md#constructor)

## Methods

- [AppendFormat(Instant, StringBuilder)](/LaunchPad/pr-preview/pr-3330/api/rocket.surgery.launchpad.nodatime/instantdatetimeoffsetpattern/methods.md#appendformat-instant-stringbuilder) : `StringBuilder` -- Formats the given value as text according to the rules of this pattern, appending to the given `Text.StringBuilder`.
- [Format(Instant)](/LaunchPad/pr-preview/pr-3330/api/rocket.surgery.launchpad.nodatime/instantdatetimeoffsetpattern/methods.md#format-instant) : `string` -- Formats the given value as text according to the rules of this pattern.
- [Parse(string)](/LaunchPad/pr-preview/pr-3330/api/rocket.surgery.launchpad.nodatime/instantdatetimeoffsetpattern/methods.md#parse-string) : `ParseResult<Instant>` -- Parses the given text value according to the rules of this pattern.
