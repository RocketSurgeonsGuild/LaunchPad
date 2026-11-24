# JsonElementScalarSerializer<TRuntime>

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.StrawberryShake](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.strawberryshake.md)
- Version: `2.0.1-pr.1304`
- Namespace: `Rocket.Surgery.LaunchPad.StrawberryShake`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/StrawberryShake/JsonElementScalarSerializer.cs)
- Inherits: `ScalarSerializer<JsonElement, TRuntime>`

Base class for JsonElement Scalar Serializers

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrawberryShake;

public abstract class JsonElementScalarSerializer<TRuntime>
    : StrawberryShake.Serialization.ScalarSerializer<System.Text.Json.JsonElement, TRuntime>
{
    // ...
}
```

## Remarks

Values are read and written with the metadata the options' resolver chain has for `TRuntime`, so no reflection is needed once a resolver for it is chained (LaunchPad's NodaTime and spatial packages chain theirs).

## Methods

- [Parse(JsonElement)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.strawberryshake/jsonelementscalarserializer-1/methods.md#parse-jsonelement) : `TRuntime` -- Parses the serialized value to the runtime representation.
