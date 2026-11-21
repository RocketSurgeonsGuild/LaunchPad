# StronglyTypedIdValueGenerator<T> Methods

- Package: [Rocket.Surgery.LaunchPad.EntityFramework](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.entityframework.md)
- Type: [StronglyTypedIdValueGenerator<T>](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.entityframework/stronglytypedidvaluegenerator-1.md)
- Kind: `Methods`
- Members: `1`

Value generator for StronglyTypedIds

<a id="next"></a>
<a id="next-entityentry"></a>

## Next(EntityEntry)

- Name: `Next(EntityEntry)`
- Modifiers: `override`
- Returns: `T`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.b95ef48c1c24b4d5eac70fa83d50ac9f4aff0224/src/EntityFramework/StronglyTypedIdValueGenerator.cs#L21)

Template method to be overridden by implementations to perform value generation.

```csharp
public class StronglyTypedIdValueGenerator<T>
{
    public override T Next(
        EntityEntry entry)
    {
        // ...
    }
}
```

## Parameters

- `entry` (`EntityEntry`)
  The change tracking entry of the entity for which the value is being generated.

## Returns

`T` -- The generated value.

## Remarks

See [EF Core value generation](https://aka.ms/efcore-docs-value-generation) for more information and examples.
