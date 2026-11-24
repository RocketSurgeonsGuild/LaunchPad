# PrimitiveBuilder<TPrimitive, TValue>

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1329`
- Namespace: `Rocket.Surgery.LaunchPad.Primitives.Advanced`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3337-merge.Sha.b3468f8f7f271d2dc63ee1d3333c2bcdd48e93a2/src/Primitives/PrimitiveBuilder.cs)

Creates a domain primitive with rules of the caller's own on top of its rules.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Primitives.Advanced;

public sealed class PrimitiveBuilder<TPrimitive, TValue>
    where TPrimitive : IDomainPrimitive<TPrimitive, TValue>
    where TValue : notnull
{
    // ...
}
```

## Type Parameters

- `TPrimitive` -- The domain primitive.
- `TValue` -- Its value.

## Methods

- [Build(TPrimitive?)](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.primitives/primitivebuilder-2/methods.md#build-tprimitive) : `bool` -- Creates the domain primitive, unless a rule rejects the value.
- [BuildOrThrow](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.primitives/primitivebuilder-2/methods.md#buildorthrow) : `TPrimitive` -- Creates the domain primitive, or throws when a rule rejects the value.
- [For](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.primitives/primitivebuilder-2/methods.md#for) : [PrimitiveBuilder<TPrimitive, TValue>](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.primitives/primitivebuilder-2.md) `static` -- Starts a builder.
- [Must(Func<TValue, bool>, string, string)](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.primitives/primitivebuilder-2/methods.md#must-func-tvalue-bool-string-string) : [PrimitiveBuilder<TPrimitive, TValue>](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.primitives/primitivebuilder-2.md) -- Adds a rule.
- [WithValue(TValue)](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.primitives/primitivebuilder-2/methods.md#withvalue-tvalue) : [PrimitiveBuilder<TPrimitive, TValue>](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.primitives/primitivebuilder-2.md) -- Sets the value.
