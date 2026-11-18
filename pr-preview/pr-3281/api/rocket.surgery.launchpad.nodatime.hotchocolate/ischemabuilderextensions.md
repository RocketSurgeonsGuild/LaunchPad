# ISchemaBuilderExtensions

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.NodaTime.HotChocolate](/LaunchPad/pr-preview/pr-3281/api/rocket.surgery.launchpad.nodatime.hotchocolate.md)
- Version: `2.0.1-pr.1256`
- Namespace: `Rocket.Surgery.LaunchPad.NodaTime.HotChocolate`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3281-merge.Sha.f470c0fad9a519f7357ef1c1f00f2210962f5308/src/NodaTime.HotChocolate/ISchemaBuilderExtensions.cs)

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

- [AddNodaTimeFilters(ISchemaBuilder)](/LaunchPad/pr-preview/pr-3281/api/rocket.surgery.launchpad.nodatime.hotchocolate/ischemabuilderextensions/methods.md#addnodatimefilters-ischemabuilder) : `ISchemaBuilder` `extension` -- Binds the NodaTime types HotChocolate has scalars for ( `OffsetDateTime`, `Duration`, `LocalDate`, `LocalDateTime` and `LocalTime`), and `Instant`, which the Joint binds to the `DateTime` scalar, to comparable filters.
