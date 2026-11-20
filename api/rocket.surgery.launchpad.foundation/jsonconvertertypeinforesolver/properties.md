# JsonConverterTypeInfoResolver Properties

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/api/rocket.surgery.launchpad.foundation.md)
- Type: [JsonConverterTypeInfoResolver](/LaunchPad/api/rocket.surgery.launchpad.foundation/jsonconvertertypeinforesolver.md)
- Kind: `Properties`
- Members: `1`

Resolves the metadata of value types written by a converter in `JsonSerializerOptions.Converters`, such as the NodaTime or the spatial types, without reflection.

<a id="types"></a>

## Types

- Name: `Types`
- Modifiers: `get`
- Returns: `IEnumerable<Type>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.6f19e0829a4004d537d23cc908a809ddcba9219c/src/Foundation/JsonConverterTypeInfoResolver.cs#L47)

The types this resolver knows.

```csharp
public IEnumerable<Type> Types { get; }
```
