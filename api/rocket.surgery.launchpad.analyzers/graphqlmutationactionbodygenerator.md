# GraphqlMutationActionBodyGenerator

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Analyzers](/LaunchPad/api/rocket.surgery.launchpad.analyzers.md)
- Version: `2.0.1-beta.321`
- Namespace: `Rocket.Surgery.LaunchPad.Analyzers`
- Target framework: `netstandard2.0`
- Implements: `IIncrementalGenerator`

Generator to automatically implement partial methods for controllers to call mediatr methods

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Analyzers;

public class GraphqlMutationActionBodyGenerator
    : Microsoft.CodeAnalysis.IIncrementalGenerator
{
    // ...
}
```

## Constructors

- [GraphqlMutationActionBodyGenerator](/LaunchPad/api/rocket.surgery.launchpad.analyzers/graphqlmutationactionbodygenerator/constructors.md#constructor)

## Methods

- [Initialize(IncrementalGeneratorInitializationContext)](/LaunchPad/api/rocket.surgery.launchpad.analyzers/graphqlmutationactionbodygenerator/methods.md#initialize-incrementalgeneratorinitializationcontext) -- Called to initialize the generator and register generation steps via callbacks on the `context`
