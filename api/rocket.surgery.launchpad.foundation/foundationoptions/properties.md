# FoundationOptions Properties

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/api/rocket.surgery.launchpad.foundation.md)
- Type: [FoundationOptions](/LaunchPad/api/rocket.surgery.launchpad.foundation/foundationoptions.md)
- Kind: `Properties`
- Members: `2`

Common foundation options

<a id="registervalidationoptionsashealthchecks"></a>

## RegisterValidationOptionsAsHealthChecks

- Name: `RegisterValidationOptionsAsHealthChecks`
- Modifiers: `nullable` `get; set`
- Returns: `bool?`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.accb8cccadcfe11a9558e1e30ee2e6f231346f67/src/Foundation/FoundationOptions.cs)

Validation options are registered as health checks instead of throwing and stopping application startup

```csharp
public bool? RegisterValidationOptionsAsHealthChecks { get; set; }
```

<a id="validatorlifetime"></a>

## ValidatorLifetime

- Name: `ValidatorLifetime`
- Modifiers: `get; set`
- Returns: `ServiceLifetime`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.accb8cccadcfe11a9558e1e30ee2e6f231346f67/src/Foundation/FoundationOptions.cs)

The Validator lifetime

```csharp
public ServiceLifetime ValidatorLifetime { get; set; }
```
