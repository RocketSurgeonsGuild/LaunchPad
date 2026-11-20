# NodaTimeSystemTextJsonSerializationExtensions

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.NodaTime](/LaunchPad/pr-preview/pr-3307/api/rocket.surgery.launchpad.nodatime.md)
- Version: `2.0.1-pr.1258`
- Namespace: `Rocket.Surgery.LaunchPad.NodaTime`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3307-merge.Sha.e1cd1b3273152fe1b84a7e7cf201a85e915d90b0/src/NodaTime/NodaTimeSystemTextJsonSerializationExtensions.cs)

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

- [TypeInfoResolver](/LaunchPad/pr-preview/pr-3307/api/rocket.surgery.launchpad.nodatime/nodatimesystemtextjsonserializationextensions/properties.md#typeinforesolver) : `IJsonTypeInfoResolver` `static` `get` -- Resolves the metadata of the NodaTime types from the converters [NodaTimeSystemTextJsonSerializationExtensions.ConfigureNodaTimeForLaunchPad(JsonSerializerOptions, IDateTimeZoneProvider)](/LaunchPad/pr-preview/pr-3307/api/rocket.surgery.launchpad.nodatime/nodatimesystemtextjsonserializationextensions/methods.md#configurenodatimeforlaunchpad-jsonserializeroptions-idatetimezoneprovider) adds, without reflection, for when they are serialized at the root or as an `Object`. [NodaTimeSystemTextJsonSerializationExtensions.ConfigureNodaTimeForLaunchPad(JsonSerializerOptions, IDateTimeZoneProvider)](/LaunchPad/pr-preview/pr-3307/api/rocket.surgery.launchpad.nodatime/nodatimesystemtextjsonserializationextensions/methods.md#configurenodatimeforlaunchpad-jsonserializeroptions-idatetimezoneprovider) chains it.

## Methods

- [ConfigureNodaTimeForLaunchPad(JsonSerializerOptions, IDateTimeZoneProvider)](/LaunchPad/pr-preview/pr-3307/api/rocket.surgery.launchpad.nodatime/nodatimesystemtextjsonserializationextensions/methods.md#configurenodatimeforlaunchpad-jsonserializeroptions-idatetimezoneprovider) : `JsonSerializerOptions` `extension` -- Configure System.Text.Json with defaults for launchpad
