# Joints

A **Joint** is a unit of application wiring: a class that registers services, adds configuration, sets up logging or telemetry, or
configures the host. Dovetail's source generator finds every Joint an application can see, its own and those of the packages and
projects it references, orders them, and writes the code that applies them. Nothing is discovered by reflection at run time.

LaunchPad used to call these _conventions_ (`Rocket.Surgery.Conventions`). Every LaunchPad convention is now a Joint, and the old
convention packages are gone.

## What a Joint looks like

A Joint implements one or more of Dovetail's Joint interfaces and is marked for export. LaunchPad's NodaTime package registers the
clock this way:

```csharp
[PublicAPI]
[DovetailExport]
[DovetailCategory(DovetailCategory.Core)]
public class TimeJoint : IServiceJoint, ISetupJoint
{
    public void Register(IDovetailContext context, IServiceCollection services)
    {
        // Try add so that unit tests can insert fakes
        services.TryAddSingleton(TimeProvider.System);
        services.TryAddSingleton<IClock>(SystemClock.Instance);
        services.TryAddSingleton<IDateTimeZoneProvider>(new DateTimeZoneCache(context.Require<NodaTimeOptions>().DateTimeZoneSource));
    }

    void ISetupJoint.Register(IDovetailContext context) => _ = context.GetOrAdd(() => new NodaTimeOptions());
}
```

- `IServiceJoint` registers services. Other interfaces cover configuration, logging, OpenTelemetry and the host builder.
- `ISetupJoint` runs first and puts defaults into the `IDovetailContext`, here the `NodaTimeOptions` an application may override.
- `[DovetailExport]` makes the Joint visible to the applications that reference the package.
- `[DovetailCategory]` puts the Joint in a category, which is part of its metadata. A Joint that declares none is in the
  `Application` category; LaunchPad's foundational Joints, like this one, declare `Core`.

## Applying the Joints

An application applies its Joints once, where it builds its host:

```csharp
var app = await WebApplication.CreateBuilder(args).ConfigureDovetail();
```

`ConfigureDovetail()` is generated into the application for its [host model](../host-models/). It runs every Joint the application
imports, in order, then builds the host.

## Where Joints come from

- **Packages.** Each LaunchPad [integration package](../integration-packages/) exports the Joints for its technology.
- **Templates.** Joints that only make sense for some host models, or only when another library is present, are
  [templates](../templates/) written into the application itself.
- **Your own code.** An application or library can define Joints of its own the same way. See
  [Writing a Joint](../../guides/writing-a-joint/).

Dovetail's documentation covers the Joint interfaces, ordering and the context in depth:
[Defining Joints](https://rocketsurgeonsguild.github.io/Dovetail/concepts/defining-joints/).