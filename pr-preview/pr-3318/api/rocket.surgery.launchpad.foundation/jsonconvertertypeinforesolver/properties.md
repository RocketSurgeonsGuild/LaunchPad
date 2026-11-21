# JsonConverterTypeInfoResolver Properties

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3318/api/rocket.surgery.launchpad.foundation.md)
- Type: [JsonConverterTypeInfoResolver](/LaunchPad/pr-preview/pr-3318/api/rocket.surgery.launchpad.foundation/jsonconvertertypeinforesolver.md)
- Kind: `Properties`
- Members: `1`

Resolves the metadata of value types written by a converter in `JsonSerializerOptions.Converters`, such as the NodaTime or the spatial types, without reflection.

<a id="types"></a>

## Types

- Name: `Types`
- Modifiers: `get`
- Returns: `IEnumerable<Type>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3318-merge.Sha.bebe34d3263b81ab1ad40e89d23fa1c640f5d677/src/Foundation/JsonConverterTypeInfoResolver.cs#L47)

The types this resolver knows.

```csharp
public IEnumerable<Type> Types { get; }
```
