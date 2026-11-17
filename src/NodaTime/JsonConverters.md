# NodaTime JSON converters: replace once NodaTime supports native AOT

LaunchPad.NodaTime ships its own System.Text.Json converters for every NodaTime type
(`SystemTextJsonCompositeNodaPatternConverter<T>` and the converters in `NodaTimeIntervalJsonConverters.cs`), instead of
[NodaTime.Serialization.SystemTextJson](https://github.com/nodatime/nodatime.serialization)'s.

**Why:** NodaTime.Serialization.SystemTextJson 1.4.0 isn't native AOT compatible. Its `Interval` and `DateInterval` converters
read and write their parts with `JsonSerializerOptions.GetConverter(Type)`, which is marked `RequiresUnreferencedCode` and
`RequiresDynamicCode`, so a native AOT publish warns (IL2026, IL3050). The assembly declares neither `IsTrimmable` nor
`IsAotCompatible`. LaunchPad's interval converters are handed the `Instant` and `LocalDate` converters directly instead.

The JSON is the same as NodaTime.Serialization.SystemTextJson's, plus the extra formats LaunchPad's pattern converters read:

| Type                                                                                                                                                | Written as                                                                                             |
| --------------------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------ |
| `Instant`, `LocalDate`, `LocalDateTime`, `LocalTime`, `Offset`, `Duration`, `Period`, `OffsetDateTime`, `OffsetDate`, `OffsetTime`, `ZonedDateTime` | a string, in the first pattern listed in `NodaTimeSystemTextJsonSerializationExtensions`               |
| `AnnualDate`, `YearMonth`                                                                                                                           | a string, `AnnualDatePattern.Iso` / `YearMonthPattern.Iso`                                             |
| `Interval`                                                                                                                                          | `{ "Start": instant, "End": instant }`, either left out when unbounded; names follow the naming policy |
| `DateInterval`                                                                                                                                      | `{ "Start": date, "End": date }`                                                                       |
| `DateTimeZone`                                                                                                                                      | its id, resolved through the application's `IDateTimeZoneProvider`                                     |

Every first pattern parses and keeps fractions of a second, so each value reads back as the value written
(`NodaTimeJsonConvertersTests.Reads_Back_Every_Type_It_Writes`). A `ZonedDateTime` is written as its local date and time,
offset and zone id (`2020-07-01T12:00:00+01 Europe/London`), NodaTime.Serialization.SystemTextJson's pattern. One difference
remains: an `OffsetDateTime` writes a whole-hour offset as `+02`, where NodaTime's RFC 3339 pattern writes `+02:00`; both are read.

**When to replace:** once NodaTime.Serialization.SystemTextJson is AOT compatible
([nodatime/nodatime.serialization#121, "Implement AOT support"](https://github.com/nodatime/nodatime.serialization/issues/121)).
Check a release declares `IsAotCompatible` and that `test/Api/Aot/verify.sh` publishes LaunchPad.NodaTime with it and no warnings.
Then:

1. Reference the package again (`Directory.Packages.props` and `Rocket.Surgery.LaunchPad.NodaTime.csproj`).
2. In `ConfigureNodaTimeForLaunchPad`, call `ConfigureForNodaTime(provider)` and keep only the converters whose formats differ
   (LaunchPad's pattern converters read more formats than NodaTime's); delete `NodaTimeIntervalJsonConverters.cs`.
3. Keep `test/NodaTime.Tests` passing; the JSON must not change.
