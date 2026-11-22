# StronglyTypedIdValueGenerator

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.EntityFramework](/LaunchPad/pr-preview/pr-3327/api/rocket.surgery.launchpad.entityframework.md)
- Version: `2.0.1-pr.1278`
- Namespace: `Rocket.Surgery.LaunchPad.EntityFramework`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3327-merge.Sha.f618e95e07fca6c7011f5e648f5b4966f567c6b1/src/EntityFramework/StronglyTypedIdValueGenerator.cs)

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

- [Create(Func<T>)](/LaunchPad/pr-preview/pr-3327/api/rocket.surgery.launchpad.entityframework/stronglytypedidvaluegenerator/methods.md#create-func-t) : `Func<IReadOnlyProperty, ITypeBase, ValueGenerator>` `static` -- Create a value generator for the given type, in the shape `PropertyBuilder.HasValueGenerator` takes.
