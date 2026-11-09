# GraphqlOptionalPropertyTrackingGenerator

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Analyzers](/LaunchPad/api/rocket.surgery.launchpad.analyzers.md)
- Version: `2.0.1-beta.322`
- Namespace: `Rocket.Surgery.LaunchPad.Analyzers`
- Target framework: `netstandard2.0`
- Implements: `IIncrementalGenerator`

A generator that is used to copy properties, fields and methods from one type onto another.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Analyzers;

public class GraphqlOptionalPropertyTrackingGenerator
    : Microsoft.CodeAnalysis.IIncrementalGenerator
{
    // ...
}
```

## Constructors

- [GraphqlOptionalPropertyTrackingGenerator](/LaunchPad/api/rocket.surgery.launchpad.analyzers/graphqloptionalpropertytrackinggenerator/constructors.md#constructor)

## Methods

- [Initialize(IncrementalGeneratorInitializationContext)](/LaunchPad/api/rocket.surgery.launchpad.analyzers/graphqloptionalpropertytrackinggenerator/methods.md#initialize-incrementalgeneratorinitializationcontext) -- Called to initialize the generator and register generation steps via callbacks on the `context`
