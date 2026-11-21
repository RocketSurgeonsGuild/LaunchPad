# PhoneNumberRules Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/api/rocket.surgery.launchpad.primitives.md)
- Type: [PhoneNumberRules](/LaunchPad/api/rocket.surgery.launchpad.primitives/phonenumberrules.md)
- Kind: `Methods`
- Members: `1`

The rules of a phone number: an ITU-T E.164 number in its international form, `+` followed by the country code and the subscriber number, such as `+14155552671`.

<a id="define"></a>

## Define

- Name: `Define`
- Modifiers: `static`
- Returns: `StrongTypeDefinition<string>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.10d9c1412b7fe8190f26ae8e00f1c59fad95faf5/src/Primitives/StrongTypes/PhoneNumber.cs#L21)

```csharp
public sealed class PhoneNumberRules
{
    public static StrongTypeDefinition<string> Define()
    {
        // ...
    }
}
```
