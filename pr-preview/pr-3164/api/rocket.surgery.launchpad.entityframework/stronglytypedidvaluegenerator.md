# StronglyTypedIdValueGenerator

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.EntityFramework](/LaunchPad/pr-preview/pr-3164/api/rocket.surgery.launchpad.entityframework.md)
- Version: `2.0.1-pr.1170`
- Namespace: `Rocket.Surgery.LaunchPad.EntityFramework`
- Target framework: `net10.0`

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

- [Create(Func<T>)](/LaunchPad/pr-preview/pr-3164/api/rocket.surgery.launchpad.entityframework/stronglytypedidvaluegenerator/methods.md#create-func-t) : `Func<IReadOnlyProperty, ITypeBase, ValueGenerator>` `static` -- Create a value generator for the given type, in the shape `PropertyBuilder.HasValueGenerator` takes.
