# SlugRules Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3291/api/rocket.surgery.launchpad.primitives.md)
- Type: [SlugRules](/LaunchPad/pr-preview/pr-3291/api/rocket.surgery.launchpad.primitives/slugrules.md)
- Kind: `Methods`
- Members: `1`

The rules of a slug, the readable part of a URL path that names a resource, such as `hello-world-2`.

<a id="define"></a>

## Define

- Name: `Define`
- Modifiers: `static`
- Returns: `StrongTypeDefinition<string>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3291-merge.Sha.13e74febbd2cc111b9b844ce764f2522bbd95912/src/Primitives/StrongTypes/Slug.cs#L23)

```csharp
public sealed class SlugRules
{
    public static StrongTypeDefinition<string> Define()
    {
        // ...
    }
}
```
