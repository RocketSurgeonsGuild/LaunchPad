# MutableGenerator Methods

- Package: [Rocket.Surgery.LaunchPad.Analyzers](/LaunchPad/pr-preview/pr-3164/api/rocket.surgery.launchpad.analyzers.md)
- Type: [MutableGenerator](/LaunchPad/pr-preview/pr-3164/api/rocket.surgery.launchpad.analyzers/mutablegenerator.md)
- Kind: `Methods`
- Members: `1`

Generator to convert an immutable type into a mutable one

<a id="initialize"></a>
<a id="initialize-incrementalgeneratorinitializationcontext"></a>

## Initialize(IncrementalGeneratorInitializationContext)

- Name: `Initialize(IncrementalGeneratorInitializationContext)`

Called to initialize the generator and register generation steps via callbacks on the `context`

```csharp
public class MutableGenerator
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
