# HexColorRules

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3335/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1284`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3335-merge.Sha.164159712cc333d1205a8448b12272b1815ca0c1/src/Primitives/StrongTypes/HexColor.cs)
- Implements: `IStrongTypeTemplate<string>`

The rules of a hexadecimal color: the CSS Color 4 §5.2 hex notation `#RGB` or `#RRGGBB`.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrongTypes;

public sealed class HexColorRules
    : SuperStrong.Types.IStrongTypeTemplate<string>
{
    // ...
}
```

## Remarks

- The forms with an alpha channel ( `#RGBA`, `#RRGGBBAA`) are rejected.
- CSS reads hex digits in either case; these rules take the lower case HTML uses for a color (the "valid lowercase simple color" of the HTML standard) and reject upper case, so equal colors are equal values. `#fff` and `#ffffff` are still different values.

## Methods

- [Define](/LaunchPad/pr-preview/pr-3335/api/rocket.surgery.launchpad.primitives/hexcolorrules/methods.md#define) : `StrongTypeDefinition<string>` `static`
