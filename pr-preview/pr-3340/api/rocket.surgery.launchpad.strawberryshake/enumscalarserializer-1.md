# EnumScalarSerializer<TRuntime>

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.StrawberryShake](/LaunchPad/pr-preview/pr-3340/api/rocket.surgery.launchpad.strawberryshake.md)
- Version: `2.0.1-pr.1300`
- Namespace: `Rocket.Surgery.LaunchPad.StrawberryShake`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3340-merge.Sha.1fd0124e0c52f2b8f0975bb6854b707a6a798292/src/StrawberryShake/EnumScalarSerializer.cs)
- Inherits: `ScalarSerializer<int, TRuntime>`

Base class for enum serializers

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrawberryShake;

public abstract class EnumScalarSerializer<TRuntime>
    : StrawberryShake.Serialization.ScalarSerializer<int, TRuntime>
    where TRuntime : struct
{
    // ...
}
```

## Methods

- [Parse(int)](/LaunchPad/pr-preview/pr-3340/api/rocket.surgery.launchpad.strawberryshake/enumscalarserializer-1/methods.md#parse-int) : `TRuntime` -- Parses the serialized value to the runtime representation.
