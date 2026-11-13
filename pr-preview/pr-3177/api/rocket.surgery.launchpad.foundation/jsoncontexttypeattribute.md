# JsonContextTypeAttribute

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3177/api/rocket.surgery.launchpad.foundation.md)
- Version: `2.0.1-pr.1182`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation`
- Target framework: `net10.0`
- Inherits: `Attribute`

Lists a type of this assembly that crosses the wire as JSON, for the JSON context LaunchPad generates in the applications that reference it.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Foundation;

public sealed class JsonContextTypeAttribute
    : System.Attribute
{
    // ...
}
```

## Remarks

The generator that ships with Rocket.Surgery.LaunchPad.Foundation writes one for every mediator request of the assembly and its response or stream item type, every `IPropertyTracking<T>` type and every type marked [IncludeInJsonContextAttribute](/LaunchPad/pr-preview/pr-3177/api/rocket.surgery.launchpad.foundation/includeinjsoncontextattribute.md). An application's build reads them from its referenced assemblies and writes a `JsonSerializerContext` that declares them all.

The context is written for the referenced assemblies because their metadata is complete: the System.Text.Json source generator sees the members other generators added to a type (property tracking, `[InheritFrom]`), which it cannot see for a type compiled in the same project.

## Constructors

- [JsonContextTypeAttribute(Type)](/LaunchPad/pr-preview/pr-3177/api/rocket.surgery.launchpad.foundation/jsoncontexttypeattribute/constructors.md#constructor-type) -- Lists a type of this assembly that crosses the wire as JSON, for the JSON context LaunchPad generates in the applications that reference it.

## Properties

- [Type](/LaunchPad/pr-preview/pr-3177/api/rocket.surgery.launchpad.foundation/jsoncontexttypeattribute/properties.md#type) : `Type` `get` -- The type.
- [TypeInfoPropertyName](/LaunchPad/pr-preview/pr-3177/api/rocket.surgery.launchpad.foundation/jsoncontexttypeattribute/properties.md#typeinfopropertyname) : `string?` `get; set` -- The name of the type's property on the generated context. The build picks a unique name from the type's name when it isn't set.
