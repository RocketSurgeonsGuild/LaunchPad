# StrongTypeFormatAttribute Constructors

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3307/api/rocket.surgery.launchpad.primitives.md)
- Type: [StrongTypeFormatAttribute](/LaunchPad/pr-preview/pr-3307/api/rocket.surgery.launchpad.primitives/strongtypeformatattribute.md)
- Kind: `Constructors`
- Members: `1`

The format a strong type's value is written in, as OpenAPI and JSON Schema name it ( `email`, `uri`, `ipv4`, ...), on the strong type or on its rules.

<a id="constructor"></a>
<a id="constructor-string"></a>

## StrongTypeFormatAttribute(string)

- Name: `Constructor(string)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3307-merge.Sha.e1cd1b3273152fe1b84a7e7cf201a85e915d90b0/src/Primitives/Descriptions/StrongTypeDescription.cs#L256-L261)

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
