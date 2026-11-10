# JsonContextTypeAttribute Properties

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/api/rocket.surgery.launchpad.primitives.md)
- Type: [JsonContextTypeAttribute](/LaunchPad/api/rocket.surgery.launchpad.primitives/jsoncontexttypeattribute.md)
- Kind: `Properties`
- Members: `2`

Lists a type of this assembly that crosses the wire as JSON, for the JSON context LaunchPad generates in the applications that reference it.

<a id="type"></a>

## Type

- Name: `Type`
- Modifiers: `get`
- Returns: `Type`

The type.

```csharp
public Type Type { get; }
```

<a id="typeinfopropertyname"></a>

## TypeInfoPropertyName

- Name: `TypeInfoPropertyName`
- Modifiers: `nullable` `get; set`
- Returns: `string?`

The name of the type's property on the generated context. The build picks a unique name from the type's name when it isn't set.

```csharp
public string? TypeInfoPropertyName { get; set; }
```
