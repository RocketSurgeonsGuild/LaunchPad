# LaunchPadJsonEnumConverters Properties

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3345/api/rocket.surgery.launchpad.foundation.md)
- Type: [LaunchPadJsonEnumConverters](/LaunchPad/pr-preview/pr-3345/api/rocket.surgery.launchpad.foundation/launchpadjsonenumconverters.md)
- Kind: `Properties`
- Members: `1`

The enum converters the `SystemTextJsonJoint` adds to the `Json.JsonSerializerOptions` it configures.

<a id="converters"></a>

## Converters

- Name: `Converters`
- Modifiers: `static` `get`
- Returns: `IReadOnlyCollection<JsonConverter>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3345-merge.Sha.a9c0325e0c8c70bc9d0569ac4df1fe5d29ffa4ec/src/Foundation/LaunchPadJsonEnumConverters.cs#L39-L44)

The converters registered so far.

```csharp
public static IReadOnlyCollection<JsonConverter> Converters { get; }
```
