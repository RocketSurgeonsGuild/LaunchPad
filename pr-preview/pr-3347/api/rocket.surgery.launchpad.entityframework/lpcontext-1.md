# LpContext<TContext>

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.EntityFramework](/LaunchPad/pr-preview/pr-3347/api/rocket.surgery.launchpad.entityframework.md)
- Version: `2.0.1-pr.1299`
- Namespace: `Rocket.Surgery.LaunchPad.EntityFramework`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3347-merge.Sha.4aab33c5f701c0001e99ba64a669e730647ab45f/src/EntityFramework/LpContext.cs)
- Inherits: `DbContext`

Default DB Context that ensures that configuration is pulled from the assembly of TContext Also enables DateTimeOffset support with Sqlite

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.EntityFramework;

public abstract class LpContext<TContext>
    : Microsoft.EntityFrameworkCore.DbContext
    where TContext : DbContext
{
    // ...
}
```
