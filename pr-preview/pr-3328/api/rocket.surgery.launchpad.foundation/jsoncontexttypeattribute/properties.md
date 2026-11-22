# JsonContextTypeAttribute Properties

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3328/api/rocket.surgery.launchpad.foundation.md)
- Type: [JsonContextTypeAttribute](/LaunchPad/pr-preview/pr-3328/api/rocket.surgery.launchpad.foundation/jsoncontexttypeattribute.md)
- Kind: `Properties`
- Members: `2`

Lists a type of this assembly that crosses the wire as JSON, for the JSON context LaunchPad generates in the applications that reference it.

<a id="type"></a>

## Type

- Name: `Type`
- Modifiers: `get`
- Returns: `Type`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3328-merge.Sha.e3c937ccb9d716ab226fa523eee028ac17c7ed68/src/Foundation/JsonContextTypeAttribute.cs#L24)

The type.

```csharp
public Type Type { get; }
```

<a id="typeinfopropertyname"></a>

## TypeInfoPropertyName

- Name: `TypeInfoPropertyName`
- Modifiers: `nullable` `get; set`
- Returns: `string?`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3328-merge.Sha.e3c937ccb9d716ab226fa523eee028ac17c7ed68/src/Foundation/JsonContextTypeAttribute.cs)

The name of the type's property on the generated context. The build picks a unique name from the type's name when it isn't set.

```csharp
public string? TypeInfoPropertyName { get; set; }
```
