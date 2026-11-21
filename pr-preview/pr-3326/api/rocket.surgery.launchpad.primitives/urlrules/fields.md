# UrlRules Fields

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3326/api/rocket.surgery.launchpad.primitives.md)
- Type: [UrlRules](/LaunchPad/pr-preview/pr-3326/api/rocket.surgery.launchpad.primitives/urlrules.md)
- Kind: `Fields`
- Members: `1`

The rules of a URL: an absolute `http` or `https` URI (RFC 3986 §4.3) with a host.

<a id="maxlength"></a>

## MaxLength

- Name: `MaxLength`
- Modifiers: `static` `const`
- Returns: `int`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3326-merge.Sha.03f97a4ece74153c5b06eb64e3df4c70a1e53a62/src/Primitives/StrongTypes/Url.cs)

The longest URL allowed.

```csharp
public const static int MaxLength
```
