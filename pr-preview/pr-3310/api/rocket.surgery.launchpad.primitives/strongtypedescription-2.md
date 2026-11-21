# StrongTypeDescription<TStrongType, TPrimitive>

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3310/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1264`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3310-merge.Sha.8860010d83b5d02194514237744998b27c36d06d/src/Primitives/Descriptions/StrongTypeDescription.cs)
- Inherits: [StrongTypeDescription](/LaunchPad/pr-preview/pr-3310/api/rocket.surgery.launchpad.primitives/strongtypedescription.md)

The description of one strong type, which knows the strong type and its value as generic arguments.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrongTypes;

public sealed class StrongTypeDescription<TStrongType, TPrimitive>
    : Rocket.Surgery.LaunchPad.StrongTypes.StrongTypeDescription
    where TStrongType : class, IStrongType<TStrongType, TPrimitive>
    where TPrimitive : notnull
{
    // ...
}
```

## Type Parameters

- `TPrimitive` -- Its value.
- `TStrongType` -- The strong type.

## Properties

- [PrimitiveType](/LaunchPad/pr-preview/pr-3310/api/rocket.surgery.launchpad.primitives/strongtypedescription-2/properties.md#primitivetype) : `Type` `get` -- The value the strong type wraps, which is what it is written as.
- [Type](/LaunchPad/pr-preview/pr-3310/api/rocket.surgery.launchpad.primitives/strongtypedescription-2/properties.md#type) : `Type` `get` -- The strong type.

## Methods

- [Accept(IStrongTypeVisitor<TResult>)](/LaunchPad/pr-preview/pr-3310/api/rocket.surgery.launchpad.primitives/strongtypedescription-2/methods.md#accept-istrongtypevisitor-tresult) : `TResult` -- Hands the strong type and its value to `visitor` as generic arguments.
