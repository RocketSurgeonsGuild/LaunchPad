# ISchemaBuilderExtensions Methods

- Package: [Rocket.Surgery.LaunchPad.NodaTime.HotChocolate](/LaunchPad/api/rocket.surgery.launchpad.nodatime.hotchocolate.md)
- Type: [ISchemaBuilderExtensions](/LaunchPad/api/rocket.surgery.launchpad.nodatime.hotchocolate/ischemabuilderextensions.md)
- Kind: `Methods`
- Members: `1`

Schema builder extensions

<a id="addnodatimefilters"></a>
<a id="addnodatimefilters-ischemabuilder"></a>

## AddNodaTimeFilters(ISchemaBuilder)

- Name: `AddNodaTimeFilters(ISchemaBuilder)`
- Modifiers: `extension`
- Returns: `ISchemaBuilder`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.afac89ac2f84551e9433e9fb435d06d6d3ff37f0/src/NodaTime.HotChocolate/ISchemaBuilderExtensions.cs#L24-L51)

Binds the NodaTime types HotChocolate has scalars for ( `OffsetDateTime`, `Duration`, `LocalDate`, `LocalDateTime` and `LocalTime`), and `Instant`, which the Joint binds to the `DateTime` scalar, to comparable filters.

```csharp
public static class ISchemaBuilderExtensions
{
    public static ISchemaBuilder AddNodaTimeFilters(
        this ISchemaBuilder schemaBuilder)
    {
        // ...
    }
}
```

## Parameters

- `schemaBuilder` (`ISchemaBuilder`)

## Remarks

The scalars themselves come from HotChocolate's own `AddNodaTime()` on the request executor builder.
