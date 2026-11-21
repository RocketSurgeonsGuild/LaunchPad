# IpAddressRules Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/api/rocket.surgery.launchpad.primitives.md)
- Type: [IpAddressRules](/LaunchPad/api/rocket.surgery.launchpad.primitives/ipaddressrules.md)
- Kind: `Methods`
- Members: `1`

The rules of an IP address in its canonical text form: IPv4 dotted decimal ( `192.0.2.1`) or IPv6 as RFC 5952 recommends ( `2001:db8::1`).

<a id="define"></a>

## Define

- Name: `Define`
- Modifiers: `static`
- Returns: `StrongTypeDefinition<string>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.be4f65663b97e5bb7c221e0ce4f3da85894aeb85/src/Primitives/StrongTypes/IpAddress.cs#L25-L27)

```csharp
public sealed class IpAddressRules
{
    public static StrongTypeDefinition<string> Define()
    {
        // ...
    }
}
```
