# HealthChecksJoint Methods

- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/pr-preview/pr-3333/api/rocket.surgery.launchpad.aspnetcore.md)
- Type: [HealthChecksJoint](/LaunchPad/pr-preview/pr-3333/api/rocket.surgery.launchpad.aspnetcore/healthchecksjoint.md)
- Kind: `Methods`
- Members: `1`

Registers the health check services and the options of LaunchPad's health check endpoints ( `MapLaunchPadHealthChecks`), bound from the [LaunchPadHealthCheckOptions.SectionName](/LaunchPad/pr-preview/pr-3333/api/rocket.surgery.launchpad.aspnetcore/launchpadhealthcheckoptions/fields.md#sectionname) configuration section. Implements the `Joints.IServiceJoint`

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3333-merge.Sha.836b758fb89040b6cc85ed7817191956e959bc1f/src/AspNetCore/Joints/HealthChecksJoint.cs#L30-L35)

Register the Service part.

```csharp
public class HealthChecksJoint
{
    public void Register(
        IDovetailContext context,
        IServiceCollection services)
    {
        // ...
    }
}
```

## Parameters

- `context` (`IDovetailContext`)
- `services` (`IServiceCollection`)
