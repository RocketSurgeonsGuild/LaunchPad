# NodaTimeOptions Properties

- Package: [Rocket.Surgery.LaunchPad.NodaTime](/LaunchPad/pr-preview/pr-3348/api/rocket.surgery.launchpad.nodatime.md)
- Type: [NodaTimeOptions](/LaunchPad/pr-preview/pr-3348/api/rocket.surgery.launchpad.nodatime/nodatimeoptions.md)
- Kind: `Properties`
- Members: `1`

The options of the NodaTime support, set on the joint context to change how it behaves.

<a id="datetimezonesource"></a>

## DateTimeZoneSource

- Name: `DateTimeZoneSource`
- Modifiers: `get; set`
- Returns: `IDateTimeZoneSource`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3348-merge.Sha.90dd4871ffef6c7ec9cccc3065d35dfa4da5b954/src/NodaTime/NodaTimeOptions.cs)

The NodaTime timezone source

```csharp
public IDateTimeZoneSource DateTimeZoneSource { get; set; }
```
