# LpContext<TContext>

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.EntityFramework](/LaunchPad/pr-preview/pr-3354/api/rocket.surgery.launchpad.entityframework.md)
- Version: `2.0.1-pr.1294`
- Namespace: `Rocket.Surgery.LaunchPad.EntityFramework`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3354-merge.Sha.770f51e280ea81405e6ea681ce5c1987ac1bab2a/src/EntityFramework/LpContext.cs)
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
