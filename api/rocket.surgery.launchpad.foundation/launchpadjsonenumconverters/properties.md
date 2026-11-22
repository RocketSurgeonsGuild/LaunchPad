# LaunchPadJsonEnumConverters Properties

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/api/rocket.surgery.launchpad.foundation.md)
- Type: [LaunchPadJsonEnumConverters](/LaunchPad/api/rocket.surgery.launchpad.foundation/launchpadjsonenumconverters.md)
- Kind: `Properties`
- Members: `1`

The enum converters the `SystemTextJsonJoint` adds to the `Json.JsonSerializerOptions` it configures.

<a id="converters"></a>

## Converters

- Name: `Converters`
- Modifiers: `static` `get`
- Returns: `IReadOnlyCollection<JsonConverter>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.58a05033e5e3d052ad687f1ec7ee0750304f817e/src/Foundation/LaunchPadJsonEnumConverters.cs#L39-L44)

The converters registered so far.

```csharp
public static IReadOnlyCollection<JsonConverter> Converters { get; }
```
