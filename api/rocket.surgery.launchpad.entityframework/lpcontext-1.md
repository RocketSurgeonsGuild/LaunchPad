# LpContext<TContext>

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.EntityFramework](/LaunchPad/api/rocket.surgery.launchpad.entityframework.md)
- Version: `2.0.1-beta.448`
- Namespace: `Rocket.Surgery.LaunchPad.EntityFramework`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.974bfd9a73f64517e8a81d86b29393f3cce75778/src/EntityFramework/LpContext.cs)
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
