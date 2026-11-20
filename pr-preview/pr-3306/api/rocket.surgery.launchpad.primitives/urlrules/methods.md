# UrlRules Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3306/api/rocket.surgery.launchpad.primitives.md)
- Type: [UrlRules](/LaunchPad/pr-preview/pr-3306/api/rocket.surgery.launchpad.primitives/urlrules.md)
- Kind: `Methods`
- Members: `1`

The rules of a URL: an absolute `http` or `https` URI (RFC 3986 §4.3) with a host.

<a id="define"></a>

## Define

- Name: `Define`
- Modifiers: `static`
- Returns: `StrongTypeDefinition<string>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3306-merge.Sha.7731056fc28c1fb31fe802623d663df67f58b451/src/Primitives/StrongTypes/Url.cs#L28-L32)

```csharp
public sealed class UrlRules
{
    public static StrongTypeDefinition<string> Define()
    {
        // ...
    }
}
```
