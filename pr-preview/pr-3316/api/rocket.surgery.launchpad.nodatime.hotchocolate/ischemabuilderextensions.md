# ISchemaBuilderExtensions

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.NodaTime.HotChocolate](/LaunchPad/pr-preview/pr-3316/api/rocket.surgery.launchpad.nodatime.hotchocolate.md)
- Version: `2.0.1-pr.1272`
- Namespace: `Rocket.Surgery.LaunchPad.NodaTime.HotChocolate`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3316-merge.Sha.4725c0a09120ecd23f44a0d7d9544708e7a3dca8/src/NodaTime.HotChocolate/ISchemaBuilderExtensions.cs)

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

- [AddNodaTimeFilters(ISchemaBuilder)](/LaunchPad/pr-preview/pr-3316/api/rocket.surgery.launchpad.nodatime.hotchocolate/ischemabuilderextensions/methods.md#addnodatimefilters-ischemabuilder) : `ISchemaBuilder` `extension` -- Binds the NodaTime types HotChocolate has scalars for ( `OffsetDateTime`, `Duration`, `LocalDate`, `LocalDateTime` and `LocalTime`), and `Instant`, which the Joint binds to the `DateTime` scalar, to comparable filters.
