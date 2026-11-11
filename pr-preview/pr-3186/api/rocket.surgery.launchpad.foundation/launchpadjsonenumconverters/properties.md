# LaunchPadJsonEnumConverters Properties

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3186/api/rocket.surgery.launchpad.foundation.md)
- Type: [LaunchPadJsonEnumConverters](/LaunchPad/pr-preview/pr-3186/api/rocket.surgery.launchpad.foundation/launchpadjsonenumconverters.md)
- Kind: `Properties`
- Members: `1`

The enum converters the `SystemTextJsonJoint` adds to the `Json.JsonSerializerOptions` it configures.

<a id="converters"></a>

## Converters

- Name: `Converters`
- Modifiers: `static` `get`
- Returns: `IReadOnlyCollection<JsonConverter>`

The converters registered so far.

```csharp
public static IReadOnlyCollection<JsonConverter> Converters { get; }
```
