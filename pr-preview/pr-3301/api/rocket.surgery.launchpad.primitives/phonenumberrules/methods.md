# PhoneNumberRules Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3301/api/rocket.surgery.launchpad.primitives.md)
- Type: [PhoneNumberRules](/LaunchPad/pr-preview/pr-3301/api/rocket.surgery.launchpad.primitives/phonenumberrules.md)
- Kind: `Methods`
- Members: `1`

The rules of a phone number: an ITU-T E.164 number in its international form, `+` followed by the country code and the subscriber number, such as `+14155552671`.

<a id="define"></a>

## Define

- Name: `Define`
- Modifiers: `static`
- Returns: `StrongTypeDefinition<string>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3301-merge.Sha.477a5d9575a97343218a11eba2a3d0088808ddab/src/Primitives/StrongTypes/PhoneNumber.cs#L21)

```csharp
public sealed class PhoneNumberRules
{
    public static StrongTypeDefinition<string> Define()
    {
        // ...
    }
}
```
