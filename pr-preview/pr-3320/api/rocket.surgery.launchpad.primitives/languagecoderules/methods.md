# LanguageCodeRules Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3320/api/rocket.surgery.launchpad.primitives.md)
- Type: [LanguageCodeRules](/LaunchPad/pr-preview/pr-3320/api/rocket.surgery.launchpad.primitives/languagecoderules.md)
- Kind: `Methods`
- Members: `1`

The rules of a language code: a BCP 47 language tag that .NET knows as a culture, such as `en`, `en-US` or `zh-Hans-CN`.

<a id="define"></a>

## Define

- Name: `Define`
- Modifiers: `static`
- Returns: `StrongTypeDefinition<string>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3320-merge.Sha.bb93b2eb5e069135a2caef17855a85e8a18d22af/src/Primitives/StrongTypes/LanguageCode.cs#L31-L33)

```csharp
public sealed class LanguageCodeRules
{
    public static StrongTypeDefinition<string> Define()
    {
        // ...
    }
}
```
