# UrlRules Fields

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3335/api/rocket.surgery.launchpad.primitives.md)
- Type: [UrlRules](/LaunchPad/pr-preview/pr-3335/api/rocket.surgery.launchpad.primitives/urlrules.md)
- Kind: `Fields`
- Members: `1`

The rules of a URL: an absolute `http` or `https` URI (RFC 3986 §4.3) with a host.

<a id="maxlength"></a>

## MaxLength

- Name: `MaxLength`
- Modifiers: `static` `const`
- Returns: `int`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3335-merge.Sha.164159712cc333d1205a8448b12272b1815ca0c1/src/Primitives/StrongTypes/Url.cs)

The longest URL allowed.

```csharp
public const static int MaxLength
```
