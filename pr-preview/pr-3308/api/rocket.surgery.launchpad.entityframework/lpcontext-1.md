# LpContext<TContext>

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.EntityFramework](/LaunchPad/pr-preview/pr-3308/api/rocket.surgery.launchpad.entityframework.md)
- Version: `2.0.1-pr.1261`
- Namespace: `Rocket.Surgery.LaunchPad.EntityFramework`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3308-merge.Sha.b5321c7138bef72baa9f203b3f824fa7207c9dcf/src/EntityFramework/LpContext.cs)
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
