# DomainPrimitiveDescription<TPrimitive, TValue> Constructors

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/api/rocket.surgery.launchpad.primitives.md)
- Type: [DomainPrimitiveDescription<TPrimitive, TValue>](/LaunchPad/api/rocket.surgery.launchpad.primitives/domainprimitivedescription-2.md)
- Kind: `Constructors`
- Members: `1`

The description of one domain primitive, which knows the domain primitive and its value as generic arguments.

<a id="constructor"></a>

## DomainPrimitiveDescription

- Name: `Constructor`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.752d0f02a7c5d0592531a44cd02f98c0fc107a63/src/Primitives/Descriptions/DomainPrimitiveDescription.cs#L16-L56)

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
