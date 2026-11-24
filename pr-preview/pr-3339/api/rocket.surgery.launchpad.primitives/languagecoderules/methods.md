# LanguageCodeRules Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3339/api/rocket.surgery.launchpad.primitives.md)
- Type: [LanguageCodeRules](/LaunchPad/pr-preview/pr-3339/api/rocket.surgery.launchpad.primitives/languagecoderules.md)
- Kind: `Methods`
- Members: `1`

The rules of a language code: a BCP 47 language tag that .NET knows as a culture, such as `en`, `en-US` or `zh-Hans-CN`.

<a id="define"></a>

## Define

- Name: `Define`
- Modifiers: `static`
- Returns: `StrongTypeDefinition<string>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3339-merge.Sha.f2b0430cc034379f71251d40ca4b57fa907c264f/src/Primitives/StrongTypes/LanguageCode.cs#L31-L33)

```csharp
public sealed class LanguageCodeRules
{
    public static StrongTypeDefinition<string> Define()
    {
        // ...
    }
}
```
