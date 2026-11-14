# IpAddressRules Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives.md)
- Type: [IpAddressRules](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/ipaddressrules.md)
- Kind: `Methods`
- Members: `1`

The rules of an IP address in its canonical text form: IPv4 dotted decimal ( `192.0.2.1`) or IPv6 as RFC 5952 recommends ( `2001:db8::1`).

<a id="define"></a>

## Define

- Name: `Define`
- Modifiers: `static`
- Returns: `StrongTypeDefinition<string>`

```csharp
public sealed class IpAddressRules
{
    public static StrongTypeDefinition<string> Define()
    {
        // ...
    }
}
```
