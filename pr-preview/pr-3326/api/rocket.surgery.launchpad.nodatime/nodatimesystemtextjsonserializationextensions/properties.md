# NodaTimeSystemTextJsonSerializationExtensions Properties

- Package: [Rocket.Surgery.LaunchPad.NodaTime](/LaunchPad/pr-preview/pr-3326/api/rocket.surgery.launchpad.nodatime.md)
- Type: [NodaTimeSystemTextJsonSerializationExtensions](/LaunchPad/pr-preview/pr-3326/api/rocket.surgery.launchpad.nodatime/nodatimesystemtextjsonserializationextensions.md)
- Kind: `Properties`
- Members: `1`

Extensions for noda time

<a id="typeinforesolver"></a>

## TypeInfoResolver

- Name: `TypeInfoResolver`
- Modifiers: `static` `get`
- Returns: `IJsonTypeInfoResolver`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3326-merge.Sha.03f97a4ece74153c5b06eb64e3df4c70a1e53a62/src/NodaTime/NodaTimeSystemTextJsonSerializationExtensions.cs#L20)

Resolves the metadata of the NodaTime types from the converters [NodaTimeSystemTextJsonSerializationExtensions.ConfigureNodaTimeForLaunchPad(JsonSerializerOptions, IDateTimeZoneProvider)](/LaunchPad/pr-preview/pr-3326/api/rocket.surgery.launchpad.nodatime/nodatimesystemtextjsonserializationextensions/methods.md#configurenodatimeforlaunchpad-jsonserializeroptions-idatetimezoneprovider) adds, without reflection, for when they are serialized at the root or as an `Object`. [NodaTimeSystemTextJsonSerializationExtensions.ConfigureNodaTimeForLaunchPad(JsonSerializerOptions, IDateTimeZoneProvider)](/LaunchPad/pr-preview/pr-3326/api/rocket.surgery.launchpad.nodatime/nodatimesystemtextjsonserializationextensions/methods.md#configurenodatimeforlaunchpad-jsonserializeroptions-idatetimezoneprovider) chains it.

```csharp
public static IJsonTypeInfoResolver TypeInfoResolver { get; }
```
