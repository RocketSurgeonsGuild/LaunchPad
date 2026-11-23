# NodaTimeStringScalarSerializer<TRuntime>

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake](/LaunchPad/api/rocket.surgery.launchpad.nodatime.strawberryshake.md)
- Version: `2.0.1-beta.449`
- Namespace: `Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.54628ef221bd1ad09c6ebc55da16ddcb5027f4c7/src/NodaTime.StrawberryShake/NodaTimeStringScalarSerializer.cs)
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

- [Parse(string)](/LaunchPad/api/rocket.surgery.launchpad.nodatime.strawberryshake/nodatimestringscalarserializer-1/methods.md#parse-string) : `TRuntime` -- Parses the serialized value to the runtime representation.
