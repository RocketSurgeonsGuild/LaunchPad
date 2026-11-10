# JsonConverterTypeInfoResolver Properties

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/api/rocket.surgery.launchpad.primitives.md)
- Type: [JsonConverterTypeInfoResolver](/LaunchPad/api/rocket.surgery.launchpad.primitives/jsonconvertertypeinforesolver.md)
- Kind: `Properties`
- Members: `1`

Resolves the metadata of value types written by a converter in `JsonSerializerOptions.Converters`, such as the NodaTime or the spatial types, without reflection.

<a id="types"></a>

## Types

- Name: `Types`
- Modifiers: `get`
- Returns: `IEnumerable<Type>`

The types this resolver knows.

```csharp
public IEnumerable<Type> Types { get; }
```
