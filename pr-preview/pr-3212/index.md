# LaunchPad — A Starting Point for .NET Applications

**One line to opt in**
Name the [LaunchPad SDK](concepts/launchpad-sdk/) next to your project's SDK. It finds the host model and adds the LaunchPad and Dovetail packages that fit.

**Wiring as Joints**
Health checks, logging, telemetry, validation and time are [Joints](concepts/joints/) that Dovetail resolves at build time. No reflection, trimming and Native AOT safe.

**Reference it, get it**
Referencing NodaTime, FluentValidation, HotChocolate or a Serilog sink is enough: the matching [integration packages](concepts/integration-packages/) and [templates](concepts/templates/) come
        with it.

**C# API reference**
Every public type of every package, generated from the build: [browse the API](api/).