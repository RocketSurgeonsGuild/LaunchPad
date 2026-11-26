# DomainPrimitiveDescription<TPrimitive, TValue> Constructors

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3345/api/rocket.surgery.launchpad.primitives.md)
- Type: [DomainPrimitiveDescription<TPrimitive, TValue>](/LaunchPad/pr-preview/pr-3345/api/rocket.surgery.launchpad.primitives/domainprimitivedescription-2.md)
- Kind: `Constructors`
- Members: `1`

The description of one domain primitive, which knows the domain primitive and its value as generic arguments.

<a id="constructor"></a>

## DomainPrimitiveDescription

- Name: `Constructor`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3345-merge.Sha.a9c0325e0c8c70bc9d0569ac4df1fe5d29ffa4ec/src/Primitives/Descriptions/DomainPrimitiveDescription.cs#L16-L56)

Creates the description; the Primitives generator writes one for every domain primitive.

```csharp
public sealed class DomainPrimitiveDescription<TPrimitive, TValue>
{
    public DomainPrimitiveDescription<TPrimitive, DomainPrimitiveDescription()
    {
        // ...
    }
}
```
