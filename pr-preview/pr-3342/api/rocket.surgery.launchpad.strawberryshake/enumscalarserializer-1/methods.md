# EnumScalarSerializer<TRuntime> Methods

- Package: [Rocket.Surgery.LaunchPad.StrawberryShake](/LaunchPad/pr-preview/pr-3342/api/rocket.surgery.launchpad.strawberryshake.md)
- Type: [EnumScalarSerializer<TRuntime>](/LaunchPad/pr-preview/pr-3342/api/rocket.surgery.launchpad.strawberryshake/enumscalarserializer-1.md)
- Kind: `Methods`
- Members: `1`

Base class for enum serializers

<a id="parse"></a>
<a id="parse-int"></a>

## Parse(int)

- Name: `Parse(int)`
- Modifiers: `override`
- Returns: `TRuntime`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3342-merge.Sha.fe29130e4542c32d064e5f519feab50b84c0905d/src/StrawberryShake/EnumScalarSerializer.cs#L21)

Parses the serialized value to the runtime representation.

```csharp
public abstract class EnumScalarSerializer<TRuntime>
{
    public override TRuntime Parse(
        int serializedValue)
    {
        // ...
    }
}
```

## Parameters

- `serializedValue` (`int`)
  The serialized value.

## Returns

`TRuntime` -- Returns the parsed runtime representation.
