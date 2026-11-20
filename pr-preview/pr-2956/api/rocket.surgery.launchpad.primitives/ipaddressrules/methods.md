# IpAddressRules Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-2956/api/rocket.surgery.launchpad.primitives.md)
- Type: [IpAddressRules](/LaunchPad/pr-preview/pr-2956/api/rocket.surgery.launchpad.primitives/ipaddressrules.md)
- Kind: `Methods`
- Members: `1`

The rules of an IP address in its canonical text form: IPv4 dotted decimal ( `192.0.2.1`) or IPv6 as RFC 5952 recommends ( `2001:db8::1`).

<a id="define"></a>

## Define

- Name: `Define`
- Modifiers: `static`
- Returns: `StrongTypeDefinition<string>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-2956-merge.Sha.e1593c4f42603ac4a494d6f792fa4b45d268d971/src/Primitives/StrongTypes/IpAddress.cs#L25-L27)

```csharp
public sealed class IpAddressRules
{
    public static StrongTypeDefinition<string> Define()
    {
        // ...
    }
}
```
