# SlugRules

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3334/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1284`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3334-merge.Sha.1bdca6582f82d4de4d3840709c4a6fa63d4c5f0c/src/Primitives/StrongTypes/Slug.cs)
- Implements: `IStrongTypeTemplate<string>`

The rules of a slug, the readable part of a URL path that names a resource, such as `hello-world-2`.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrongTypes;

public sealed class SlugRules
    : SuperStrong.Types.IStrongTypeTemplate<string>
{
    // ...
}
```

## Remarks

No standard defines a slug. These rules take the form that needs no percent-encoding in any part of a URI (RFC 3986 §2.3):

- Lower-case ASCII letters and digits, in groups separated by single hyphens; no leading or trailing hyphen.
- At most 100 characters.

## Methods

- [Define](/LaunchPad/pr-preview/pr-3334/api/rocket.surgery.launchpad.primitives/slugrules/methods.md#define) : `StrongTypeDefinition<string>` `static`

## Fields

- [MaxLength](/LaunchPad/pr-preview/pr-3334/api/rocket.surgery.launchpad.primitives/slugrules/fields.md#maxlength) : `int` `static` -- The longest slug allowed.
