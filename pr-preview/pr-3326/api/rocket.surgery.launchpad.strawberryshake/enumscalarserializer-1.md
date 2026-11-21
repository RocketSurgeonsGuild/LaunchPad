# EnumScalarSerializer<TRuntime>

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.StrawberryShake](/LaunchPad/pr-preview/pr-3326/api/rocket.surgery.launchpad.strawberryshake.md)
- Version: `2.0.1-pr.1278`
- Namespace: `Rocket.Surgery.LaunchPad.StrawberryShake`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3326-merge.Sha.03f97a4ece74153c5b06eb64e3df4c70a1e53a62/src/StrawberryShake/EnumScalarSerializer.cs)
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

- [Parse(int)](/LaunchPad/pr-preview/pr-3326/api/rocket.surgery.launchpad.strawberryshake/enumscalarserializer-1/methods.md#parse-int) : `TRuntime` -- Parses the serialized value to the runtime representation.
