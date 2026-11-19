# NodaTimeOptions Properties

- Package: [Rocket.Surgery.LaunchPad.NodaTime](/LaunchPad/pr-preview/pr-3299/api/rocket.surgery.launchpad.nodatime.md)
- Type: [NodaTimeOptions](/LaunchPad/pr-preview/pr-3299/api/rocket.surgery.launchpad.nodatime/nodatimeoptions.md)
- Kind: `Properties`
- Members: `1`

The options of the NodaTime support, set on the joint context to change how it behaves.

<a id="datetimezonesource"></a>

## DateTimeZoneSource

- Name: `DateTimeZoneSource`
- Modifiers: `get; set`
- Returns: `IDateTimeZoneSource`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3299-merge.Sha.307aa7d68ee152c9493e88e21694727a4cdb86e8/src/NodaTime/NodaTimeOptions.cs)

The NodaTime timezone source

```csharp
public IDateTimeZoneSource DateTimeZoneSource { get; set; }
```
