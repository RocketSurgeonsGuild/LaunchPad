# ISchemaBuilderExtensions

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.NodaTime.HotChocolate](/LaunchPad/pr-preview/pr-3309/api/rocket.surgery.launchpad.nodatime.hotchocolate.md)
- Version: `2.0.1-pr.1264`
- Namespace: `Rocket.Surgery.LaunchPad.NodaTime.HotChocolate`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3309-merge.Sha.4aede2174ef26f3e0e8e5f30e59b3cf34a9c1971/src/NodaTime.HotChocolate/ISchemaBuilderExtensions.cs)

Schema builder extensions

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.NodaTime.HotChocolate;

public static class ISchemaBuilderExtensions
{
    // ...
}
```

## Methods

- [AddNodaTimeFilters(ISchemaBuilder)](/LaunchPad/pr-preview/pr-3309/api/rocket.surgery.launchpad.nodatime.hotchocolate/ischemabuilderextensions/methods.md#addnodatimefilters-ischemabuilder) : `ISchemaBuilder` `extension` -- Binds the NodaTime types HotChocolate has scalars for ( `OffsetDateTime`, `Duration`, `LocalDate`, `LocalDateTime` and `LocalTime`), and `Instant`, which the Joint binds to the `DateTime` scalar, to comparable filters.
