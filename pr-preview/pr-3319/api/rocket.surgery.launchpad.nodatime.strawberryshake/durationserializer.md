# DurationSerializer

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake](/LaunchPad/pr-preview/pr-3319/api/rocket.surgery.launchpad.nodatime.strawberryshake.md)
- Version: `2.0.1-pr.1271`
- Namespace: `Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3319-merge.Sha.db5ef919c4f6a03a7f33e4672dc70757d169d5a4/src/NodaTime.StrawberryShake/DurationSerializer.cs)
- Inherits: `ScalarSerializer<string, Duration>`

General purpose Duration Serializer

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake;

public class DurationSerializer
    : StrawberryShake.Serialization.ScalarSerializer<string, NodaTime.Duration>
{
    // ...
}
```

## Remarks

Reads and writes the ISO 8601 durations of the `Duration` scalar of HotChocolate 16: `[-]P[nW][nD][T[nH][nM][n[.f]S]]`, with an optional sign on each component. Years and months have no fixed length, so a duration with them is rejected.

## Constructors

- [DurationSerializer](/LaunchPad/pr-preview/pr-3319/api/rocket.surgery.launchpad.nodatime.strawberryshake/durationserializer/constructors.md#constructor) -- Create a new DurationSerializer

## Methods

- [Parse(string)](/LaunchPad/pr-preview/pr-3319/api/rocket.surgery.launchpad.nodatime.strawberryshake/durationserializer/methods.md#parse-string) : `Duration` -- Parses the serialized value to the runtime representation.
