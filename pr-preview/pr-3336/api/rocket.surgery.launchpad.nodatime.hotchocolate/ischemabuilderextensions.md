# ISchemaBuilderExtensions

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.NodaTime.HotChocolate](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.nodatime.hotchocolate.md)
- Version: `2.0.1-pr.1292`
- Namespace: `Rocket.Surgery.LaunchPad.NodaTime.HotChocolate`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3336-merge.Sha.9ec91d866bcac66448bc0f439b45a4702f1b73d3/src/NodaTime.HotChocolate/ISchemaBuilderExtensions.cs)

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

- [AddNodaTimeFilters(ISchemaBuilder)](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.nodatime.hotchocolate/ischemabuilderextensions/methods.md#addnodatimefilters-ischemabuilder) : `ISchemaBuilder` `extension` -- Binds the NodaTime types HotChocolate has scalars for ( `OffsetDateTime`, `Duration`, `LocalDate`, `LocalDateTime` and `LocalTime`), and `Instant`, which the Joint binds to the `DateTime` scalar, to comparable filters.
