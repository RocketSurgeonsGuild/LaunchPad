# StronglyTypedIdValueGenerator

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.EntityFramework](/LaunchPad/pr-preview/pr-2602/api/rocket.surgery.launchpad.entityframework.md)
- Version: `2.0.1-pr.1260`
- Namespace: `Rocket.Surgery.LaunchPad.EntityFramework`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-2602-merge.Sha.022b9cee91d8927e916725e8a1c1717558619950/src/EntityFramework/StronglyTypedIdValueGenerator.cs)

Factory a StronglyTypedId value generator

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.EntityFramework;

public static class StronglyTypedIdValueGenerator
{
    // ...
}
```

## Methods

- [Create(Func<T>)](/LaunchPad/pr-preview/pr-2602/api/rocket.surgery.launchpad.entityframework/stronglytypedidvaluegenerator/methods.md#create-func-t) : `Func<IReadOnlyProperty, ITypeBase, ValueGenerator>` `static` -- Create a value generator for the given type, in the shape `PropertyBuilder.HasValueGenerator` takes.
