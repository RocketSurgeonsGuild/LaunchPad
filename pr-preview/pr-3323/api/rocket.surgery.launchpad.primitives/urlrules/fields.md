# UrlRules Fields

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3323/api/rocket.surgery.launchpad.primitives.md)
- Type: [UrlRules](/LaunchPad/pr-preview/pr-3323/api/rocket.surgery.launchpad.primitives/urlrules.md)
- Kind: `Fields`
- Members: `1`

The rules of a URL: an absolute `http` or `https` URI (RFC 3986 §4.3) with a host.

<a id="maxlength"></a>

## MaxLength

- Name: `MaxLength`
- Modifiers: `static` `const`
- Returns: `int`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3323-merge.Sha.848d7df41fb82a23ce0738e64978a35c934d7558/src/Primitives/StrongTypes/Url.cs)

The longest URL allowed.

```csharp
public const static int MaxLength
```
