# PhoneNumberRules Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3292/api/rocket.surgery.launchpad.primitives.md)
- Type: [PhoneNumberRules](/LaunchPad/pr-preview/pr-3292/api/rocket.surgery.launchpad.primitives/phonenumberrules.md)
- Kind: `Methods`
- Members: `1`

The rules of a phone number: an ITU-T E.164 number in its international form, `+` followed by the country code and the subscriber number, such as `+14155552671`.

<a id="define"></a>

## Define

- Name: `Define`
- Modifiers: `static`
- Returns: `StrongTypeDefinition<string>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3292-merge.Sha.a3b1ee2389c613a51b34add57c7d3d20cf877d5b/src/Primitives/StrongTypes/PhoneNumber.cs#L21)

```csharp
public sealed class PhoneNumberRules
{
    public static StrongTypeDefinition<string> Define()
    {
        // ...
    }
}
```
