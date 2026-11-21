# EnumScalarSerializer<TRuntime>

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.StrawberryShake](/LaunchPad/pr-preview/pr-3311/api/rocket.surgery.launchpad.strawberryshake.md)
- Version: `2.0.1-pr.1268`
- Namespace: `Rocket.Surgery.LaunchPad.StrawberryShake`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3311-merge.Sha.1b8c4d06515a0e43de2a920227f55c17896fc638/src/StrawberryShake/EnumScalarSerializer.cs)
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

- [Parse(int)](/LaunchPad/pr-preview/pr-3311/api/rocket.surgery.launchpad.strawberryshake/enumscalarserializer-1/methods.md#parse-int) : `TRuntime` -- Parses the serialized value to the runtime representation.
