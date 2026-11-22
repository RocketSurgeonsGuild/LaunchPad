# NodaTimeStringScalarSerializer<TRuntime>

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake](/LaunchPad/pr-preview/pr-3334/api/rocket.surgery.launchpad.nodatime.strawberryshake.md)
- Version: `2.0.1-pr.1284`
- Namespace: `Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3334-merge.Sha.1bdca6582f82d4de4d3840709c4a6fa63d4c5f0c/src/NodaTime.StrawberryShake/NodaTimeStringScalarSerializer.cs)
- Inherits: `ScalarSerializer<string, TRuntime>`

Shared base class for NodaTime string serializers

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake;

public abstract class NodaTimeStringScalarSerializer<TRuntime>
    : StrawberryShake.Serialization.ScalarSerializer<string, TRuntime>
{
    // ...
}
```

## Type Parameters

- `TRuntime` -- The NodaTime type.

## Methods

- [Parse(string)](/LaunchPad/pr-preview/pr-3334/api/rocket.surgery.launchpad.nodatime.strawberryshake/nodatimestringscalarserializer-1/methods.md#parse-string) : `TRuntime` -- Parses the serialized value to the runtime representation.
