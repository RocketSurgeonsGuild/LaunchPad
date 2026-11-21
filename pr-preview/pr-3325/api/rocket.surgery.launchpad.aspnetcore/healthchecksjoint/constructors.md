# HealthChecksJoint Constructors

- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.aspnetcore.md)
- Type: [HealthChecksJoint](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.aspnetcore/healthchecksjoint.md)
- Kind: `Constructors`
- Members: `1`

Registers the health check services and the options of LaunchPad's health check endpoints ( `MapLaunchPadHealthChecks`), bound from the [LaunchPadHealthCheckOptions.SectionName](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.aspnetcore/launchpadhealthcheckoptions/fields.md#sectionname) configuration section. Implements the `Joints.IServiceJoint`

<a id="constructor"></a>

## HealthChecksJoint

- Name: `Constructor`

```csharp
public class HealthChecksJoint
{
    public HealthChecksJoint()
    {
        // ...
    }
}
```
