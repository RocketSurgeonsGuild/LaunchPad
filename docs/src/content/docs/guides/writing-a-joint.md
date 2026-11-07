---
title: Writing a Joint
description: Add wiring of your own to an application or a library as a Joint that Dovetail applies with LaunchPad's.
---

Wiring of your own, in an application or in a library it references, is a [Joint](../../concepts/joints/) like LaunchPad's. Dovetail
finds it at build time and `ConfigureDovetail()` applies it with the rest.

## A service Joint

```csharp
using Joinery.Dovetail;
using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Joints;
using Microsoft.Extensions.DependencyInjection;

namespace MyCompany.Orders;

[DovetailExport]
public sealed class OrdersJoint : IServiceJoint
{
    public void Register(IDovetailContext context, IServiceCollection services) =>
        services.AddSingleton<IOrderRepository, OrderRepository>();
}
```

- In an **application**, the Joint is applied by the application's own `ConfigureDovetail()`.
- In a **library**, `[DovetailExport]` exports it, and every application that references the library applies it. The library needs
  `Joinery.Dovetail` (or the [LaunchPad SDK](../../concepts/launchpad-sdk/), which adds it when the library references a
  technology LaunchPad supports).

## Options from the context

A Joint can read settings from the `IDovetailContext`, which the application can fill before the Joints run. LaunchPad's own
Joints do this for their options; `TimeJoint` reads `NodaTimeOptions`:

```csharp
public sealed class OrdersJoint : IServiceJoint, ISetupJoint
{
    // runs before any IServiceJoint, so the default is there for every Joint that asks
    void ISetupJoint.Register(IDovetailContext context) => _ = context.GetOrAdd(() => new OrdersOptions());

    public void Register(IDovetailContext context, IServiceCollection services) =>
        services.AddSingleton(context.Require<OrdersOptions>());
}
```

## Ordering and categories

Dovetail orders the Joints it applies. A Joint can declare what it must run after, and a `[DovetailCategory]`; one without a
category is in the `Application` category. Dovetail's documentation covers the interfaces, ordering, categories and injecting
dependencies: [Defining Joints](https://rocketsurgeonsguild.github.io/Dovetail/concepts/defining-joints/).

## Testing a Joint

LaunchPad's own tests check a Joint through Dovetail's generated exports: that it is exported, in the right category, and
registers what it should. See
[Unit tests](https://rocketsurgeonsguild.github.io/Dovetail/concepts/unit-tests/) in Dovetail's documentation.
