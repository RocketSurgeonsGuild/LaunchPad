# StronglyTypedIdValueGenerator<T> Methods

- Package: [Rocket.Surgery.LaunchPad.EntityFramework](/LaunchPad/pr-preview/pr-3322/api/rocket.surgery.launchpad.entityframework.md)
- Type: [StronglyTypedIdValueGenerator<T>](/LaunchPad/pr-preview/pr-3322/api/rocket.surgery.launchpad.entityframework/stronglytypedidvaluegenerator-1.md)
- Kind: `Methods`
- Members: `1`

Value generator for StronglyTypedIds

<a id="next"></a>
<a id="next-entityentry"></a>

## Next(EntityEntry)

- Name: `Next(EntityEntry)`
- Modifiers: `override`
- Returns: `T`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3322-merge.Sha.b1912d7b3f7d0067c507ed7826db18ff6ade1c34/src/EntityFramework/StronglyTypedIdValueGenerator.cs#L21)

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
