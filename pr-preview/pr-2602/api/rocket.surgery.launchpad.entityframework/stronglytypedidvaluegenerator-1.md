# StronglyTypedIdValueGenerator<T>

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.EntityFramework](/LaunchPad/pr-preview/pr-2602/api/rocket.surgery.launchpad.entityframework.md)
- Version: `2.0.1-pr.1259`
- Namespace: `Rocket.Surgery.LaunchPad.EntityFramework`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-2602-merge.Sha.f387c871fa01cb1c7bfc78f0117912dc30d931f2/src/EntityFramework/StronglyTypedIdValueGenerator.cs)
- Inherits: `ValueGenerator<T>`

Value generator for StronglyTypedIds

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.EntityFramework;

public class StronglyTypedIdValueGenerator<T>
    : Microsoft.EntityFrameworkCore.ValueGeneration.ValueGenerator<T>
{
    // ...
}
```

## Properties

- [GeneratesTemporaryValues](/LaunchPad/pr-preview/pr-2602/api/rocket.surgery.launchpad.entityframework/stronglytypedidvaluegenerator-1/properties.md#generatestemporaryvalues) : `bool` `get` -- Gets a value indicating whether the values generated are temporary (i.e they should be replaced by database generated values when the entity is saved) or are permanent (i.e. the generated values should be saved to the database).

## Methods

- [Next(EntityEntry)](/LaunchPad/pr-preview/pr-2602/api/rocket.surgery.launchpad.entityframework/stronglytypedidvaluegenerator-1/methods.md#next-entityentry) : `T` -- Template method to be overridden by implementations to perform value generation.
