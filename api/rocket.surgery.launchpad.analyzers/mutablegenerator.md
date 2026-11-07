# MutableGenerator

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Analyzers](/LaunchPad/api/rocket.surgery.launchpad.analyzers.md)
- Version: `2.0.1-beta.315`
- Namespace: `Rocket.Surgery.LaunchPad.Analyzers`
- Target framework: `netstandard2.0`
- Implements: `IIncrementalGenerator`

Generator to convert an immutable type into a mutable one

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Analyzers;

public class MutableGenerator
    : Microsoft.CodeAnalysis.IIncrementalGenerator
{
    // ...
}
```

## Constructors

- [MutableGenerator](/LaunchPad/api/rocket.surgery.launchpad.analyzers/mutablegenerator/constructors.md#constructor)

## Methods

- [Initialize(IncrementalGeneratorInitializationContext)](/LaunchPad/api/rocket.surgery.launchpad.analyzers/mutablegenerator/methods.md#initialize-incrementalgeneratorinitializationcontext) -- Called to initialize the generator and register generation steps via callbacks on the `context`
