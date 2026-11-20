# EnumScalarSerializer<TRuntime>

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.StrawberryShake](/LaunchPad/pr-preview/pr-3302/api/rocket.surgery.launchpad.strawberryshake.md)
- Version: `2.0.1-pr.1260`
- Namespace: `Rocket.Surgery.LaunchPad.StrawberryShake`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3302-merge.Sha.ef1fddc80f2867bf510546b2a05239671f3f33c1/src/StrawberryShake/EnumScalarSerializer.cs)
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

- [Parse(int)](/LaunchPad/pr-preview/pr-3302/api/rocket.surgery.launchpad.strawberryshake/enumscalarserializer-1/methods.md#parse-int) : `TRuntime` -- Parses the serialized value to the runtime representation.
