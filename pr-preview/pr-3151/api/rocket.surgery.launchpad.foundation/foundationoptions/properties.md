# FoundationOptions Properties

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3151/api/rocket.surgery.launchpad.foundation.md)
- Type: [FoundationOptions](/LaunchPad/pr-preview/pr-3151/api/rocket.surgery.launchpad.foundation/foundationoptions.md)
- Kind: `Properties`
- Members: `3`

Common foundation options

<a id="mediatorlifetime"></a>

## MediatorLifetime

- Name: `MediatorLifetime`
- Modifiers: `get; set`
- Returns: `ServiceLifetime`

The Mediator lifetime

```csharp
public ServiceLifetime MediatorLifetime { get; set; }
```

<a id="registervalidationoptionsashealthchecks"></a>

## RegisterValidationOptionsAsHealthChecks

- Name: `RegisterValidationOptionsAsHealthChecks`
- Modifiers: `nullable` `get; set`
- Returns: `bool?`

Validation options are registered as health checks instead of throwing and stopping application startup

```csharp
public bool? RegisterValidationOptionsAsHealthChecks { get; set; }
```

<a id="validatorlifetime"></a>

## ValidatorLifetime

- Name: `ValidatorLifetime`
- Modifiers: `get; set`
- Returns: `ServiceLifetime`

The Validator lifetime

```csharp
public ServiceLifetime ValidatorLifetime { get; set; }
```
