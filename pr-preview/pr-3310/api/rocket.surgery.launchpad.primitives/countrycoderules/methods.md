# CountryCodeRules Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3310/api/rocket.surgery.launchpad.primitives.md)
- Type: [CountryCodeRules](/LaunchPad/pr-preview/pr-3310/api/rocket.surgery.launchpad.primitives/countrycoderules.md)
- Kind: `Methods`
- Members: `1`

The rules of a country code: an officially assigned ISO 3166-1 alpha-2 code, such as `US` or `DE`.

<a id="define"></a>

## Define

- Name: `Define`
- Modifiers: `static`
- Returns: `StrongTypeDefinition<string>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3310-merge.Sha.8860010d83b5d02194514237744998b27c36d06d/src/Primitives/StrongTypes/CountryCode.cs#L31-L37)

```csharp
public sealed class CountryCodeRules
{
    public static StrongTypeDefinition<string> Define()
    {
        // ...
    }
}
```
