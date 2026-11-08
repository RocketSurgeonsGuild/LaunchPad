# InheritFromGenerator

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Analyzers](/LaunchPad/pr-preview/pr-3151/api/rocket.surgery.launchpad.analyzers.md)
- Version: `2.0.1-pr.1180`
- Namespace: `Rocket.Surgery.LaunchPad.Analyzers`
- Target framework: `netstandard2.0`
- Implements: `IIncrementalGenerator`

A generator that is used to copy properties, fields and methods from one type onto another.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Analyzers;

public class InheritFromGenerator
    : Microsoft.CodeAnalysis.IIncrementalGenerator
{
    // ...
}
```

## Constructors

- [InheritFromGenerator](/LaunchPad/pr-preview/pr-3151/api/rocket.surgery.launchpad.analyzers/inheritfromgenerator/constructors.md#constructor)

## Methods

- [Initialize(IncrementalGeneratorInitializationContext)](/LaunchPad/pr-preview/pr-3151/api/rocket.surgery.launchpad.analyzers/inheritfromgenerator/methods.md#initialize-incrementalgeneratorinitializationcontext) -- Called to initialize the generator and register generation steps via callbacks on the `context`
