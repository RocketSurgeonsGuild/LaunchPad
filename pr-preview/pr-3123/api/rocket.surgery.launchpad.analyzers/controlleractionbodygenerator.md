# ControllerActionBodyGenerator

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Analyzers](/LaunchPad/pr-preview/pr-3123/api/rocket.surgery.launchpad.analyzers.md)
- Version: `2.0.1-pr.1152`
- Namespace: `Rocket.Surgery.LaunchPad.Analyzers`
- Target framework: `netstandard2.0`
- Implements: `IIncrementalGenerator`

Generator to automatically implement partial methods for controllers to call mediatr methods

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Analyzers;

public class ControllerActionBodyGenerator
    : Microsoft.CodeAnalysis.IIncrementalGenerator
{
    // ...
}
```

## Constructors

- [ControllerActionBodyGenerator](/LaunchPad/pr-preview/pr-3123/api/rocket.surgery.launchpad.analyzers/controlleractionbodygenerator/constructors.md#constructor)

## Methods

- [Initialize(IncrementalGeneratorInitializationContext)](/LaunchPad/pr-preview/pr-3123/api/rocket.surgery.launchpad.analyzers/controlleractionbodygenerator/methods.md#initialize-incrementalgeneratorinitializationcontext) -- Called to initialize the generator and register generation steps via callbacks on the `context`
