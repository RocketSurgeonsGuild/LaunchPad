# UrlRules

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-beta.344`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net10.0`
- Implements: `IStrongTypeTemplate<string>`

The rules of a URL: an absolute `http` or `https` URI (RFC 3986 §4.3) with a host.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrongTypes;

public sealed class UrlRules
    : SuperStrong.Types.IStrongTypeTemplate<string>
{
    // ...
}
```

## Remarks

- The scheme is written in lower case, the form RFC 3986 §3.1 says to produce; `HTTP://` is rejected.
- Only the characters RFC 3986 allows in a URI: unreserved and reserved characters, and `%` followed by two hexadecimal digits. Spaces and other characters must already be percent-encoded, and internationalized host names written as A-labels.
- At most 8000 characters, the length RFC 9110 §4.1 recommends every recipient supports.
- The value is kept as given; it is not normalized.

## Methods

- [Define](/LaunchPad/api/rocket.surgery.launchpad.primitives/urlrules/methods.md#define) : `StrongTypeDefinition<string>` `static`

## Fields

- [MaxLength](/LaunchPad/api/rocket.surgery.launchpad.primitives/urlrules/fields.md#maxlength) : `int` `static` -- The longest URL allowed.
