# DomainPrimitiveDescription<TPrimitive, TValue>

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-beta.458`
- Namespace: `Rocket.Surgery.LaunchPad.Primitives.Descriptions`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.6a854476dc50a3b95bf56cf9e7b0d45ee9f0bfa7/src/Primitives/Descriptions/DomainPrimitiveDescription.cs)
- Inherits: [DomainPrimitiveDescription](/LaunchPad/api/rocket.surgery.launchpad.primitives/domainprimitivedescription.md)

The description of one domain primitive, which knows the domain primitive and its value as generic arguments.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Primitives.Descriptions;

public sealed class DomainPrimitiveDescription<TPrimitive, TValue>
    : Rocket.Surgery.LaunchPad.Primitives.Descriptions.DomainPrimitiveDescription
    where TPrimitive : IDomainPrimitive<TPrimitive, TValue>
    where TValue : notnull
{
    // ...
}
```

## Type Parameters

- `TPrimitive` -- The domain primitive.
- `TValue` -- Its value.

## Constructors

- [DomainPrimitiveDescription](/LaunchPad/api/rocket.surgery.launchpad.primitives/domainprimitivedescription-2/constructors.md#constructor) -- Creates the description; the Primitives generator writes one for every domain primitive.

## Properties

- [Type](/LaunchPad/api/rocket.surgery.launchpad.primitives/domainprimitivedescription-2/properties.md#type) : `Type` `get` -- The domain primitive.
- [ValueType](/LaunchPad/api/rocket.surgery.launchpad.primitives/domainprimitivedescription-2/properties.md#valuetype) : `Type` `get` -- The value the domain primitive holds, which is what it is written as.

## Methods

- [Accept(IDomainPrimitiveVisitor<TResult>)](/LaunchPad/api/rocket.surgery.launchpad.primitives/domainprimitivedescription-2/methods.md#accept-idomainprimitivevisitor-tresult) : `TResult` -- Hands the domain primitive and its value to `visitor` as generic arguments.
