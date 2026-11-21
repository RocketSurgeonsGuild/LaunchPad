# NodaTimeSystemTextJsonSerializationExtensions Properties

- Package: [Rocket.Surgery.LaunchPad.NodaTime](/LaunchPad/api/rocket.surgery.launchpad.nodatime.md)
- Type: [NodaTimeSystemTextJsonSerializationExtensions](/LaunchPad/api/rocket.surgery.launchpad.nodatime/nodatimesystemtextjsonserializationextensions.md)
- Kind: `Properties`
- Members: `1`

Extensions for noda time

<a id="typeinforesolver"></a>

## TypeInfoResolver

- Name: `TypeInfoResolver`
- Modifiers: `static` `get`
- Returns: `IJsonTypeInfoResolver`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.5eb9940f30f7eb1f8bff840daf4429d170c460a1/src/NodaTime/NodaTimeSystemTextJsonSerializationExtensions.cs#L20)

Resolves the metadata of the NodaTime types from the converters [NodaTimeSystemTextJsonSerializationExtensions.ConfigureNodaTimeForLaunchPad(JsonSerializerOptions, IDateTimeZoneProvider)](/LaunchPad/api/rocket.surgery.launchpad.nodatime/nodatimesystemtextjsonserializationextensions/methods.md#configurenodatimeforlaunchpad-jsonserializeroptions-idatetimezoneprovider) adds, without reflection, for when they are serialized at the root or as an `Object`. [NodaTimeSystemTextJsonSerializationExtensions.ConfigureNodaTimeForLaunchPad(JsonSerializerOptions, IDateTimeZoneProvider)](/LaunchPad/api/rocket.surgery.launchpad.nodatime/nodatimesystemtextjsonserializationextensions/methods.md#configurenodatimeforlaunchpad-jsonserializeroptions-idatetimezoneprovider) chains it.

```csharp
public static IJsonTypeInfoResolver TypeInfoResolver { get; }
```
