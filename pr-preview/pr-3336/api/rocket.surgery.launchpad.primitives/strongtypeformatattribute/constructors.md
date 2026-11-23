# StrongTypeFormatAttribute Constructors

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.primitives.md)
- Type: [StrongTypeFormatAttribute](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.primitives/strongtypeformatattribute.md)
- Kind: `Constructors`
- Members: `1`

The format a strong type's value is written in, as OpenAPI and JSON Schema name it ( `email`, `uri`, `ipv4`, ...), on the strong type or on its rules.

<a id="constructor"></a>
<a id="constructor-string"></a>

## StrongTypeFormatAttribute(string)

- Name: `Constructor(string)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3336-merge.Sha.28f891b4cf6958b8ac5addf5a93485615fecf518/src/Primitives/Descriptions/StrongTypeDescription.cs#L256-L261)

The format a strong type's value is written in, as OpenAPI and JSON Schema name it ( `email`, `uri`, `ipv4`, ...), on the strong type or on its rules.

```csharp
public sealed class StrongTypeFormatAttribute
{
    public StrongTypeFormatAttribute(
        string format)
    {
        // ...
    }
}
```

## Parameters

- `format` (`string`)
  The format.
