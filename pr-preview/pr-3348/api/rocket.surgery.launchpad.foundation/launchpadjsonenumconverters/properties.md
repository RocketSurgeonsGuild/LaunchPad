# LaunchPadJsonEnumConverters Properties

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3348/api/rocket.surgery.launchpad.foundation.md)
- Type: [LaunchPadJsonEnumConverters](/LaunchPad/pr-preview/pr-3348/api/rocket.surgery.launchpad.foundation/launchpadjsonenumconverters.md)
- Kind: `Properties`
- Members: `1`

The enum converters the `SystemTextJsonJoint` adds to the `Json.JsonSerializerOptions` it configures.

<a id="converters"></a>

## Converters

- Name: `Converters`
- Modifiers: `static` `get`
- Returns: `IReadOnlyCollection<JsonConverter>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3348-merge.Sha.90dd4871ffef6c7ec9cccc3065d35dfa4da5b954/src/Foundation/LaunchPadJsonEnumConverters.cs#L39-L44)

The converters registered so far.

```csharp
public static IReadOnlyCollection<JsonConverter> Converters { get; }
```
