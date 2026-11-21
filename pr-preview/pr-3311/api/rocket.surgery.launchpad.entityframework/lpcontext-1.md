# LpContext<TContext>

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.EntityFramework](/LaunchPad/pr-preview/pr-3311/api/rocket.surgery.launchpad.entityframework.md)
- Version: `2.0.1-pr.1268`
- Namespace: `Rocket.Surgery.LaunchPad.EntityFramework`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3311-merge.Sha.1b8c4d06515a0e43de2a920227f55c17896fc638/src/EntityFramework/LpContext.cs)
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
