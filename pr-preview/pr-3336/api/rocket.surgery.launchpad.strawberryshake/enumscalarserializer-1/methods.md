# EnumScalarSerializer<TRuntime> Methods

- Package: [Rocket.Surgery.LaunchPad.StrawberryShake](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.strawberryshake.md)
- Type: [EnumScalarSerializer<TRuntime>](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.strawberryshake/enumscalarserializer-1.md)
- Kind: `Methods`
- Members: `1`

Base class for enum serializers

<a id="parse"></a>
<a id="parse-int"></a>

## Parse(int)

- Name: `Parse(int)`
- Modifiers: `override`
- Returns: `TRuntime`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3336-merge.Sha.52b728c7d35f7e5541d5238c567b2b39c033d3c7/src/StrawberryShake/EnumScalarSerializer.cs#L21)

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
