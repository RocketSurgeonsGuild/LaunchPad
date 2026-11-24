# EnumScalarSerializer<TRuntime>

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.StrawberryShake](/LaunchPad/api/rocket.surgery.launchpad.strawberryshake.md)
- Version: `2.0.1-beta.453`
- Namespace: `Rocket.Surgery.LaunchPad.StrawberryShake`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.3675b8bfd84082dc0cd600fe8d8fc9c14e97833a/src/StrawberryShake/EnumScalarSerializer.cs)
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

- [Parse(int)](/LaunchPad/api/rocket.surgery.launchpad.strawberryshake/enumscalarserializer-1/methods.md#parse-int) : `TRuntime` -- Parses the serialized value to the runtime representation.
