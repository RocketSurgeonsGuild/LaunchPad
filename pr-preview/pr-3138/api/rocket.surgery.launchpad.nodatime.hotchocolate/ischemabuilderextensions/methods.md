# ISchemaBuilderExtensions Methods

- Package: [Rocket.Surgery.LaunchPad.NodaTime.HotChocolate](/LaunchPad/pr-preview/pr-3138/api/rocket.surgery.launchpad.nodatime.hotchocolate.md)
- Type: [ISchemaBuilderExtensions](/LaunchPad/pr-preview/pr-3138/api/rocket.surgery.launchpad.nodatime.hotchocolate/ischemabuilderextensions.md)
- Kind: `Methods`
- Members: `1`

Schema builder extensions

<a id="addnodatime"></a>
<a id="addnodatime-ischemabuilder-idatetimezoneprovider"></a>

## AddNodaTime(ISchemaBuilder, IDateTimeZoneProvider)

- Name: `AddNodaTime(ISchemaBuilder, IDateTimeZoneProvider)`
- Modifiers: `extension`
- Returns: `ISchemaBuilder`

Add nodatime types to HotChocolate

```csharp
public static class ISchemaBuilderExtensions
{
    public static ISchemaBuilder AddNodaTime(
        this ISchemaBuilder schemaBuilder,
        IDateTimeZoneProvider dateTimeZoneProvider)
    {
        // ...
    }
}
```

## Parameters

- `schemaBuilder` (`ISchemaBuilder`)
- `dateTimeZoneProvider` (`IDateTimeZoneProvider`)
