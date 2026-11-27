# NodaTimeSystemTextJsonSerializationExtensions

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.NodaTime](/LaunchPad/api/rocket.surgery.launchpad.nodatime.md)
- Version: `2.0.1-beta.458`
- Namespace: `Rocket.Surgery.LaunchPad.NodaTime`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.6a854476dc50a3b95bf56cf9e7b0d45ee9f0bfa7/src/NodaTime/NodaTimeSystemTextJsonSerializationExtensions.cs)

Extensions for noda time

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.NodaTime;

public static class NodaTimeSystemTextJsonSerializationExtensions
{
    // ...
}
```

## Properties

- [TypeInfoResolver](/LaunchPad/api/rocket.surgery.launchpad.nodatime/nodatimesystemtextjsonserializationextensions/properties.md#typeinforesolver) : `IJsonTypeInfoResolver` `static` `get` -- Resolves the metadata of the NodaTime types from the converters [NodaTimeSystemTextJsonSerializationExtensions.ConfigureNodaTimeForLaunchPad(JsonSerializerOptions, IDateTimeZoneProvider)](/LaunchPad/api/rocket.surgery.launchpad.nodatime/nodatimesystemtextjsonserializationextensions/methods.md#configurenodatimeforlaunchpad-jsonserializeroptions-idatetimezoneprovider) adds, without reflection, for when they are serialized at the root or as an `Object`. [NodaTimeSystemTextJsonSerializationExtensions.ConfigureNodaTimeForLaunchPad(JsonSerializerOptions, IDateTimeZoneProvider)](/LaunchPad/api/rocket.surgery.launchpad.nodatime/nodatimesystemtextjsonserializationextensions/methods.md#configurenodatimeforlaunchpad-jsonserializeroptions-idatetimezoneprovider) chains it.

## Methods

- [ConfigureNodaTimeForLaunchPad(JsonSerializerOptions, IDateTimeZoneProvider)](/LaunchPad/api/rocket.surgery.launchpad.nodatime/nodatimesystemtextjsonserializationextensions/methods.md#configurenodatimeforlaunchpad-jsonserializeroptions-idatetimezoneprovider) : `JsonSerializerOptions` `extension` -- Configure System.Text.Json with defaults for launchpad
