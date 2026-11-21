# EmailAddressRules Fields

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3319/api/rocket.surgery.launchpad.primitives.md)
- Type: [EmailAddressRules](/LaunchPad/pr-preview/pr-3319/api/rocket.surgery.launchpad.primitives/emailaddressrules.md)
- Kind: `Fields`
- Members: `2`

The rules of an email address: an RFC 5322 `addr-spec` in the form RFC 5321 delivers mail to.

<a id="maxlength"></a>

## MaxLength

- Name: `MaxLength`
- Modifiers: `static` `const`
- Returns: `int`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3319-merge.Sha.db5ef919c4f6a03a7f33e4672dc70757d169d5a4/src/Primitives/StrongTypes/EmailAddress.cs)

The longest email address RFC 5321 can deliver to.

```csharp
public const static int MaxLength
```

<a id="maxlocalpartlength"></a>

## MaxLocalPartLength

- Name: `MaxLocalPartLength`
- Modifiers: `static` `const`
- Returns: `int`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3319-merge.Sha.db5ef919c4f6a03a7f33e4672dc70757d169d5a4/src/Primitives/StrongTypes/EmailAddress.cs)

The longest local part RFC 5321 allows.

```csharp
public const static int MaxLocalPartLength
```
