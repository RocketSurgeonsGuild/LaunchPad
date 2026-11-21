# StronglyTypedIdValueGenerator<T>

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.EntityFramework](/LaunchPad/pr-preview/pr-3309/api/rocket.surgery.launchpad.entityframework.md)
- Version: `2.0.1-pr.1264`
- Namespace: `Rocket.Surgery.LaunchPad.EntityFramework`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3309-merge.Sha.4aede2174ef26f3e0e8e5f30e59b3cf34a9c1971/src/EntityFramework/StronglyTypedIdValueGenerator.cs)
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

- [GeneratesTemporaryValues](/LaunchPad/pr-preview/pr-3309/api/rocket.surgery.launchpad.entityframework/stronglytypedidvaluegenerator-1/properties.md#generatestemporaryvalues) : `bool` `get` -- Gets a value indicating whether the values generated are temporary (i.e they should be replaced by database generated values when the entity is saved) or are permanent (i.e. the generated values should be saved to the database).

## Methods

- [Next(EntityEntry)](/LaunchPad/pr-preview/pr-3309/api/rocket.surgery.launchpad.entityframework/stronglytypedidvaluegenerator-1/methods.md#next-entityentry) : `T` -- Template method to be overridden by implementations to perform value generation.
