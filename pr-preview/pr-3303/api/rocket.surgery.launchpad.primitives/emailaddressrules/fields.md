# EmailAddressRules Fields

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3303/api/rocket.surgery.launchpad.primitives.md)
- Type: [EmailAddressRules](/LaunchPad/pr-preview/pr-3303/api/rocket.surgery.launchpad.primitives/emailaddressrules.md)
- Kind: `Fields`
- Members: `2`

The rules of an email address: an RFC 5322 `addr-spec` in the form RFC 5321 delivers mail to.

<a id="maxlength"></a>

## MaxLength

- Name: `MaxLength`
- Modifiers: `static` `const`
- Returns: `int`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3303-merge.Sha.af4f47555b5ed577aca4ba95e6332df625343e98/src/Primitives/StrongTypes/EmailAddress.cs)

The longest email address RFC 5321 can deliver to.

```csharp
public const static int MaxLength
```

<a id="maxlocalpartlength"></a>

## MaxLocalPartLength

- Name: `MaxLocalPartLength`
- Modifiers: `static` `const`
- Returns: `int`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3303-merge.Sha.af4f47555b5ed577aca4ba95e6332df625343e98/src/Primitives/StrongTypes/EmailAddress.cs)

The longest local part RFC 5321 allows.

```csharp
public const static int MaxLocalPartLength
```
