# PropertyTrackingGenerator Methods

- Package: [Rocket.Surgery.LaunchPad.Analyzers](/LaunchPad/pr-preview/pr-3141/api/rocket.surgery.launchpad.analyzers.md)
- Type: [PropertyTrackingGenerator](/LaunchPad/pr-preview/pr-3141/api/rocket.surgery.launchpad.analyzers/propertytrackinggenerator.md)
- Kind: `Methods`
- Members: `1`

A generator that is used to copy properties, fields and methods from one type onto another.

<a id="initialize"></a>
<a id="initialize-incrementalgeneratorinitializationcontext"></a>

## Initialize(IncrementalGeneratorInitializationContext)

- Name: `Initialize(IncrementalGeneratorInitializationContext)`

Called to initialize the generator and register generation steps via callbacks on the `context`

```csharp
public class PropertyTrackingGenerator
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
