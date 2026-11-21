# UrlRules Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3316/api/rocket.surgery.launchpad.primitives.md)
- Type: [UrlRules](/LaunchPad/pr-preview/pr-3316/api/rocket.surgery.launchpad.primitives/urlrules.md)
- Kind: `Methods`
- Members: `1`

The rules of a URL: an absolute `http` or `https` URI (RFC 3986 §4.3) with a host.

<a id="define"></a>

## Define

- Name: `Define`
- Modifiers: `static`
- Returns: `StrongTypeDefinition<string>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3316-merge.Sha.4725c0a09120ecd23f44a0d7d9544708e7a3dca8/src/Primitives/StrongTypes/Url.cs#L28-L32)

```csharp
public sealed class UrlRules
{
    public static StrongTypeDefinition<string> Define()
    {
        // ...
    }
}
```
