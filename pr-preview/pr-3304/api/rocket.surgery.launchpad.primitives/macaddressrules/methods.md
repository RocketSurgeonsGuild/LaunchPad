# MacAddressRules Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3304/api/rocket.surgery.launchpad.primitives.md)
- Type: [MacAddressRules](/LaunchPad/pr-preview/pr-3304/api/rocket.surgery.launchpad.primitives/macaddressrules.md)
- Kind: `Methods`
- Members: `1`

The rules of a MAC address, an IEEE EUI-48, in the IEEE 802 hexadecimal representation: six groups of two upper-case hexadecimal digits separated by hyphens, such as `00-1A-2B-3C-4D-5E`.

<a id="define"></a>

## Define

- Name: `Define`
- Modifiers: `static`
- Returns: `StrongTypeDefinition<string>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3304-merge.Sha.ea4753f3bbfef2a34e4933806cae0d39e6651456/src/Primitives/StrongTypes/MacAddress.cs#L24)

```csharp
public sealed class MacAddressRules
{
    public static StrongTypeDefinition<string> Define()
    {
        // ...
    }
}
```
