# EmailAddressRules

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1182`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net10.0`
- Implements: `IStrongTypeTemplate<string>`

The rules of an email address: an RFC 5322 `addr-spec` in the form RFC 5321 delivers mail to.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrongTypes;

public sealed class EmailAddressRules
    : SuperStrong.Types.IStrongTypeTemplate<string>
{
    // ...
}
```

## Remarks

- At most 254 characters (the RFC 5321 §4.5.3.1.3 path limit of 256 less its angle brackets), with a local part of at most 64 (RFC 5321 §4.5.3.1.1).
- The local part is a `dot-atom` (RFC 5322 §3.4.1, §3.2.3): letters, digits and `!#$%&'*+/=?^_`{|}~-`, in runs separated by single dots. Quoted local parts, comments and the obsolete syntax are rejected.
- The domain is a host name of at least two labels (RFC 1035 §2.3.1, RFC 1123 §2.1): each 1 to 63 letters, digits and hyphens that neither starts nor ends with a hyphen, and a top-level label that starts with a letter (RFC 3696 §2). Address literals ( `[192.0.2.1]`) and single-label domains ( `user@localhost`) are rejected.
- ASCII only: an internationalized domain is written as its A-label ( `xn--`), and internationalized local parts (RFC 6531) are rejected.
- The value is kept as given. The local part is case-sensitive (RFC 5321 §2.4), so addresses that differ only in case are different values.

## Methods

- [Define](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/emailaddressrules/methods.md#define) : `StrongTypeDefinition<string>` `static`

## Fields

- [MaxLength](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/emailaddressrules/fields.md#maxlength) : `int` `static` -- The longest email address RFC 5321 can deliver to.
- [MaxLocalPartLength](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.primitives/emailaddressrules/fields.md#maxlocalpartlength) : `int` `static` -- The longest local part RFC 5321 allows.
