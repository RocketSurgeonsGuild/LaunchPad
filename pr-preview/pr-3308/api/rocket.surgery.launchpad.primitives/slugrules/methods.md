# SlugRules Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3308/api/rocket.surgery.launchpad.primitives.md)
- Type: [SlugRules](/LaunchPad/pr-preview/pr-3308/api/rocket.surgery.launchpad.primitives/slugrules.md)
- Kind: `Methods`
- Members: `1`

The rules of a slug, the readable part of a URL path that names a resource, such as `hello-world-2`.

<a id="define"></a>

## Define

- Name: `Define`
- Modifiers: `static`
- Returns: `StrongTypeDefinition<string>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3308-merge.Sha.b5321c7138bef72baa9f203b3f824fa7207c9dcf/src/Primitives/StrongTypes/Slug.cs#L23)

```csharp
public sealed class SlugRules
{
    public static StrongTypeDefinition<string> Define()
    {
        // ...
    }
}
```
