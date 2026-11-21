# StrongTypeFormatAttribute Constructors

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3309/api/rocket.surgery.launchpad.primitives.md)
- Type: [StrongTypeFormatAttribute](/LaunchPad/pr-preview/pr-3309/api/rocket.surgery.launchpad.primitives/strongtypeformatattribute.md)
- Kind: `Constructors`
- Members: `1`

The format a strong type's value is written in, as OpenAPI and JSON Schema name it ( `email`, `uri`, `ipv4`, ...), on the strong type or on its rules.

<a id="constructor"></a>
<a id="constructor-string"></a>

## StrongTypeFormatAttribute(string)

- Name: `Constructor(string)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3309-merge.Sha.4aede2174ef26f3e0e8e5f30e59b3cf34a9c1971/src/Primitives/Descriptions/StrongTypeDescription.cs#L256-L261)

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
