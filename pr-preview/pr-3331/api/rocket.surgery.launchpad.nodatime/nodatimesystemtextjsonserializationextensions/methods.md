# NodaTimeSystemTextJsonSerializationExtensions Methods

- Package: [Rocket.Surgery.LaunchPad.NodaTime](/LaunchPad/pr-preview/pr-3331/api/rocket.surgery.launchpad.nodatime.md)
- Type: [NodaTimeSystemTextJsonSerializationExtensions](/LaunchPad/pr-preview/pr-3331/api/rocket.surgery.launchpad.nodatime/nodatimesystemtextjsonserializationextensions.md)
- Kind: `Methods`
- Members: `1`

Extensions for noda time

<a id="configurenodatimeforlaunchpad"></a>
<a id="configurenodatimeforlaunchpad-jsonserializeroptions-idatetimezoneprovider"></a>

## ConfigureNodaTimeForLaunchPad(JsonSerializerOptions, IDateTimeZoneProvider)

- Name: `ConfigureNodaTimeForLaunchPad(JsonSerializerOptions, IDateTimeZoneProvider)`
- Modifiers: `extension`
- Returns: `JsonSerializerOptions`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3331-merge.Sha.4e9f7c28a9b18bc67da869f4b85f2484ef7df001/src/NodaTime/NodaTimeSystemTextJsonSerializationExtensions.cs#L37-L124)

Configure System.Text.Json with defaults for launchpad

```csharp
public static class NodaTimeSystemTextJsonSerializationExtensions
{
    public static JsonSerializerOptions ConfigureNodaTimeForLaunchPad(
        this JsonSerializerOptions options,
        IDateTimeZoneProvider dateTimeZoneProvider)
    {
        // ...
    }
}
```

## Parameters

- `options` (`JsonSerializerOptions`)
- `dateTimeZoneProvider` (`IDateTimeZoneProvider`)

## Remarks

Adds LaunchPad's converter for every NodaTime type, replacing any converter for it already in the options, and [NodaTimeSystemTextJsonSerializationExtensions.TypeInfoResolver](/LaunchPad/pr-preview/pr-3331/api/rocket.surgery.launchpad.nodatime/nodatimesystemtextjsonserializationextensions/properties.md#typeinforesolver) to the options' `JsonSerializerOptions.TypeInfoResolverChain` so the NodaTime types also resolve in a trimmed or native AOT application. No reflection based resolver is added: options that should reflect over other types need one of their own. The converters are LaunchPad's own, not NodaTime.Serialization.SystemTextJson's, which isn't native AOT compatible yet (see JsonConverters.md in the package's source); the JSON is the same.
