# Rocket.Surgery.LaunchPad.NodaTime

- Package: `Rocket.Surgery.LaunchPad.NodaTime`
- Version: `2.0.1-beta.352`
- Target framework: `net10.0`
- NuGet: [Rocket.Surgery.LaunchPad.NodaTime](https://www.nuget.org/packages/Rocket.Surgery.LaunchPad.NodaTime)

## Namespaces

### Rocket.Surgery.LaunchPad.NodaTime

- [InstantDateTimeOffsetPattern](/LaunchPad/api/rocket.surgery.launchpad.nodatime/instantdatetimeoffsetpattern.md) -- `class` -- A pattern used to create an instant from a serialized DateTimeOffset value
- [NodaTimeLoggerConfigurationExtensions](/LaunchPad/api/rocket.surgery.launchpad.nodatime/nodatimeloggerconfigurationextensions.md) -- `class` `static` -- Adds the Destructure.NodaTimeTypes() extension to `LoggerConfiguration`.
- [NodaTimeOptions](/LaunchPad/api/rocket.surgery.launchpad.nodatime/nodatimeoptions.md) -- `class` -- The options of the NodaTime support, set on the joint context to change how it behaves.
- [NodaTimeSystemTextJsonSerializationExtensions](/LaunchPad/api/rocket.surgery.launchpad.nodatime/nodatimesystemtextjsonserializationextensions.md) -- `class` `static` -- Extensions for noda time
- [SystemTextJsonCompositeNodaPatternConverter<T>](/LaunchPad/api/rocket.surgery.launchpad.nodatime/systemtextjsoncompositenodapatternconverter-1.md) -- `class` `sealed` -- A JSON converter for types which can be represented by a single string value, parsed or formatted from an `Text.IPattern`1`.

### Rocket.Surgery.LaunchPad.NodaTime.Joints

- [NodaTimeSerilogJoint](/LaunchPad/api/rocket.surgery.launchpad.nodatime/nodatimeserilogjoint.md) -- `class` -- A joint that destructures the NodaTime types in the logger Dovetail creates, the way [NodaTimeLoggerConfigurationExtensions.NodaTimeTypes(LoggerDestructuringConfiguration)](/LaunchPad/api/rocket.surgery.launchpad.nodatime/nodatimeloggerconfigurationextensions/methods.md#nodatimetypes-loggerdestructuringconfiguration) does. Implements the `Joints.ISerilogJoint`
- [NodaTimeSystemTextJsonJoint](/LaunchPad/api/rocket.surgery.launchpad.nodatime/nodatimesystemtextjsonjoint.md) -- `class` -- A joint that configures `Json.JsonSerializerOptions` with the NodaTime converters, using the `IDateTimeZoneProvider` that is registered. Implements the `Joints.IServiceJoint`
- [TimeJoint](/LaunchPad/api/rocket.surgery.launchpad.nodatime/timejoint.md) -- `class` -- A joint that registers the clock, time provider and time zone provider. Implements the `Joints.IServiceJoint`
