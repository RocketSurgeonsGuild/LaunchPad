# EmailAddressRules Fields

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/api/rocket.surgery.launchpad.primitives.md)
- Type: [EmailAddressRules](/LaunchPad/api/rocket.surgery.launchpad.primitives/emailaddressrules.md)
- Kind: `Fields`
- Members: `2`

The rules of an email address: an RFC 5322 `addr-spec` in the form RFC 5321 delivers mail to.

<a id="maxlength"></a>

## MaxLength

- Name: `MaxLength`
- Modifiers: `static` `const`
- Returns: `int`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.bc6e38a474db052319ef3b044454738f3a433de2/src/Primitives/StrongTypes/EmailAddress.cs)

The longest email address RFC 5321 can deliver to.

```csharp
public const static int MaxLength
```

<a id="maxlocalpartlength"></a>

## MaxLocalPartLength

- Name: `MaxLocalPartLength`
- Modifiers: `static` `const`
- Returns: `int`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.bc6e38a474db052319ef3b044454738f3a433de2/src/Primitives/StrongTypes/EmailAddress.cs)

The longest local part RFC 5321 allows.

```csharp
public const static int MaxLocalPartLength
```
