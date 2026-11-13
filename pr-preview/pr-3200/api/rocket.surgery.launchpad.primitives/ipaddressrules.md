# IpAddressRules

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3200/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1182`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net10.0`
- Implements: `IStrongTypeTemplate<string>`

The rules of an IP address in its canonical text form: IPv4 dotted decimal ( `192.0.2.1`) or IPv6 as RFC 5952 recommends ( `2001:db8::1`).

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrongTypes;

public sealed class IpAddressRules
    : SuperStrong.Types.IStrongTypeTemplate<string>
{
    // ...
}
```

## Remarks

- The value must be exactly what `IPAddress.ToString` writes for it, so each address has one value: IPv4 without leading zeros or shortened forms ( `127.1`), IPv6 in lower case with the longest run of zero groups compressed (RFC 5952 §4). `2001:DB8::1` and `2001:0db8:0:0:0:0:0:1` are rejected.
- IPv6 zone identifiers ( `fe80::1%eth0`) are rejected; they only mean something on the host that wrote them.
- A prefix length (CIDR notation) is not an address and is rejected.

## Methods

- [Define](/LaunchPad/pr-preview/pr-3200/api/rocket.surgery.launchpad.primitives/ipaddressrules/methods.md#define) : `StrongTypeDefinition<string>` `static`
