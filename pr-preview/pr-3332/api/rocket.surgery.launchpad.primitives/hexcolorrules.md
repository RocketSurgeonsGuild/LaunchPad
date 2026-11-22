# HexColorRules

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3332/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1284`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3332-merge.Sha.108cb13ce492322587639698c87355d3b8fcee1b/src/Primitives/StrongTypes/HexColor.cs)
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

- [Define](/LaunchPad/pr-preview/pr-3332/api/rocket.surgery.launchpad.primitives/hexcolorrules/methods.md#define) : `StrongTypeDefinition<string>` `static`
