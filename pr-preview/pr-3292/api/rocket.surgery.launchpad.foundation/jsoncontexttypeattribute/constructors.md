# JsonContextTypeAttribute Constructors

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3292/api/rocket.surgery.launchpad.foundation.md)
- Type: [JsonContextTypeAttribute](/LaunchPad/pr-preview/pr-3292/api/rocket.surgery.launchpad.foundation/jsoncontexttypeattribute.md)
- Kind: `Constructors`
- Members: `1`

Lists a type of this assembly that crosses the wire as JSON, for the JSON context LaunchPad generates in the applications that reference it.

<a id="constructor"></a>
<a id="constructor-type"></a>

## JsonContextTypeAttribute(Type)

- Name: `Constructor(Type)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3292-merge.Sha.a3b1ee2389c613a51b34add57c7d3d20cf877d5b/src/Foundation/JsonContextTypeAttribute.cs#L21-L24)

Lists a type of this assembly that crosses the wire as JSON, for the JSON context LaunchPad generates in the applications that reference it.

```csharp
public sealed class JsonContextTypeAttribute
{
    public JsonContextTypeAttribute(
        Type type)
    {
        // ...
    }
}
```

## Parameters

- `type` (`Type`)
  The type.

## Remarks

The generator that ships with Rocket.Surgery.LaunchPad.Foundation writes one for every mediator request of the assembly and its response or stream item type, every `IPropertyTracking<T>` type and every type marked [IncludeInJsonContextAttribute](/LaunchPad/pr-preview/pr-3292/api/rocket.surgery.launchpad.foundation/includeinjsoncontextattribute.md). An application's build reads them from its referenced assemblies and writes a `JsonSerializerContext` that declares them all.

The context is written for the referenced assemblies because their metadata is complete: the System.Text.Json source generator sees the members other generators added to a type (property tracking, `[InheritFrom]`), which it cannot see for a type compiled in the same project.
