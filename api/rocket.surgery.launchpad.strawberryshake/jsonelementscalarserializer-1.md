# JsonElementScalarSerializer<TRuntime>

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.StrawberryShake](/LaunchPad/api/rocket.surgery.launchpad.strawberryshake.md)
- Version: `2.0.1-beta.452`
- Namespace: `Rocket.Surgery.LaunchPad.StrawberryShake`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.166abe0de80666ff26996d1f2603a1f6ba81c14c/src/StrawberryShake/JsonElementScalarSerializer.cs)
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

- [Parse(JsonElement)](/LaunchPad/api/rocket.surgery.launchpad.strawberryshake/jsonelementscalarserializer-1/methods.md#parse-jsonelement) : `TRuntime` -- Parses the serialized value to the runtime representation.
