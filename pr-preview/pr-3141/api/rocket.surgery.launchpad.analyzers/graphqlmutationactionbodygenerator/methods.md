# GraphqlMutationActionBodyGenerator Methods

- Package: [Rocket.Surgery.LaunchPad.Analyzers](/LaunchPad/pr-preview/pr-3141/api/rocket.surgery.launchpad.analyzers.md)
- Type: [GraphqlMutationActionBodyGenerator](/LaunchPad/pr-preview/pr-3141/api/rocket.surgery.launchpad.analyzers/graphqlmutationactionbodygenerator.md)
- Kind: `Methods`
- Members: `1`

Generator to automatically implement partial methods for controllers to call mediatr methods

<a id="initialize"></a>
<a id="initialize-incrementalgeneratorinitializationcontext"></a>

## Initialize(IncrementalGeneratorInitializationContext)

- Name: `Initialize(IncrementalGeneratorInitializationContext)`

Called to initialize the generator and register generation steps via callbacks on the `context`

```csharp
public class GraphqlMutationActionBodyGenerator
{
    public void Initialize(
        IncrementalGeneratorInitializationContext context)
    {
        // ...
    }
}
```

## Parameters

- `context` (`IncrementalGeneratorInitializationContext`)
  The `CodeAnalysis.IncrementalGeneratorInitializationContext` to register callbacks on
