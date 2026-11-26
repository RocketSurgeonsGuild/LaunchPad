# NodaTimeStringScalarSerializer<TRuntime>

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake](/LaunchPad/pr-preview/pr-3344/api/rocket.surgery.launchpad.nodatime.strawberryshake.md)
- Version: `2.0.1-pr.1290`
- Namespace: `Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3344-merge.Sha.14b61fb07a6a75a780072fb07f6d7e8b006bf2cf/src/NodaTime.StrawberryShake/NodaTimeStringScalarSerializer.cs)
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

- [Parse(string)](/LaunchPad/pr-preview/pr-3344/api/rocket.surgery.launchpad.nodatime.strawberryshake/nodatimestringscalarserializer-1/methods.md#parse-string) : `TRuntime` -- Parses the serialized value to the runtime representation.
