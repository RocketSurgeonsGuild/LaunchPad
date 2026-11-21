# StronglyTypedIdValueGenerator

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.EntityFramework](/LaunchPad/api/rocket.surgery.launchpad.entityframework.md)
- Version: `2.0.1-beta.441`
- Namespace: `Rocket.Surgery.LaunchPad.EntityFramework`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.be4f65663b97e5bb7c221e0ce4f3da85894aeb85/src/EntityFramework/StronglyTypedIdValueGenerator.cs)

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

- [Create(Func<T>)](/LaunchPad/api/rocket.surgery.launchpad.entityframework/stronglytypedidvaluegenerator/methods.md#create-func-t) : `Func<IReadOnlyProperty, ITypeBase, ValueGenerator>` `static` -- Create a value generator for the given type, in the shape `PropertyBuilder.HasValueGenerator` takes.
