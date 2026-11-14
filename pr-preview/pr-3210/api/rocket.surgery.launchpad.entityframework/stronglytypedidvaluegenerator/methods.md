# StronglyTypedIdValueGenerator Methods

- Package: [Rocket.Surgery.LaunchPad.EntityFramework](/LaunchPad/pr-preview/pr-3210/api/rocket.surgery.launchpad.entityframework.md)
- Type: [StronglyTypedIdValueGenerator](/LaunchPad/pr-preview/pr-3210/api/rocket.surgery.launchpad.entityframework/stronglytypedidvaluegenerator.md)
- Kind: `Methods`
- Members: `1`

Factory a StronglyTypedId value generator

<a id="create"></a>
<a id="create-func-t"></a>

## Create(Func<T>)

- Name: `Create(Func<T>)`
- Modifiers: `static`
- Returns: `Func<IReadOnlyProperty, ITypeBase, ValueGenerator>`

Create a value generator for the given type, in the shape `PropertyBuilder.HasValueGenerator` takes.

```csharp
public static class StronglyTypedIdValueGenerator
{
    public static Func<IReadOnlyProperty, ITypeBase, ValueGenerator> Create<T>(
        Func<T> factory)
    {
        // ...
    }
}
```

## Parameters

- `factory` (`Func<T>`)
  Creates each new id.

## Returns

`Func<IReadOnlyProperty, ITypeBase, ValueGenerator>` -- The value generator factory.
