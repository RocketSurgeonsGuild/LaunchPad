# NodaTimeOptions Properties

- Package: [Rocket.Surgery.LaunchPad.NodaTime](/LaunchPad/pr-preview/pr-3349/api/rocket.surgery.launchpad.nodatime.md)
- Type: [NodaTimeOptions](/LaunchPad/pr-preview/pr-3349/api/rocket.surgery.launchpad.nodatime/nodatimeoptions.md)
- Kind: `Properties`
- Members: `1`

The options of the NodaTime support, set on the joint context to change how it behaves.

<a id="datetimezonesource"></a>

## DateTimeZoneSource

- Name: `DateTimeZoneSource`
- Modifiers: `get; set`
- Returns: `IDateTimeZoneSource`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3349-merge.Sha.70168524ff777a6c4fd1a4bf57611d64095a2c00/src/NodaTime/NodaTimeOptions.cs)

The NodaTime timezone source

```csharp
public IDateTimeZoneSource DateTimeZoneSource { get; set; }
```
