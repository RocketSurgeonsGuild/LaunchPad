# EnumScalarSerializer<TRuntime> Methods

- Package: [Rocket.Surgery.LaunchPad.StrawberryShake](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.strawberryshake.md)
- Type: [EnumScalarSerializer<TRuntime>](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.strawberryshake/enumscalarserializer-1.md)
- Kind: `Methods`
- Members: `1`

Base class for enum serializers

<a id="parse"></a>
<a id="parse-int"></a>

## Parse(int)

- Name: `Parse(int)`
- Modifiers: `override`
- Returns: `TRuntime`

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
