# NodaTimeOptions Properties

- Package: [Rocket.Surgery.LaunchPad.NodaTime](/LaunchPad/pr-preview/pr-3329/api/rocket.surgery.launchpad.nodatime.md)
- Type: [NodaTimeOptions](/LaunchPad/pr-preview/pr-3329/api/rocket.surgery.launchpad.nodatime/nodatimeoptions.md)
- Kind: `Properties`
- Members: `1`

The options of the NodaTime support, set on the joint context to change how it behaves.

<a id="datetimezonesource"></a>

## DateTimeZoneSource

- Name: `DateTimeZoneSource`
- Modifiers: `get; set`
- Returns: `IDateTimeZoneSource`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3329-merge.Sha.a2bf3f292dae7cdf0854fdc32bf25e9839e7110d/src/NodaTime/NodaTimeOptions.cs)

The NodaTime timezone source

```csharp
public IDateTimeZoneSource DateTimeZoneSource { get; set; }
```
