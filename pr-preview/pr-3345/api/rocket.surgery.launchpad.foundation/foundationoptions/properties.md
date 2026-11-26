# FoundationOptions Properties

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3345/api/rocket.surgery.launchpad.foundation.md)
- Type: [FoundationOptions](/LaunchPad/pr-preview/pr-3345/api/rocket.surgery.launchpad.foundation/foundationoptions.md)
- Kind: `Properties`
- Members: `2`

Common foundation options

<a id="registervalidationoptionsashealthchecks"></a>

## RegisterValidationOptionsAsHealthChecks

- Name: `RegisterValidationOptionsAsHealthChecks`
- Modifiers: `nullable` `get; set`
- Returns: `bool?`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3345-merge.Sha.a9c0325e0c8c70bc9d0569ac4df1fe5d29ffa4ec/src/Foundation/FoundationOptions.cs)

Validation options are registered as health checks instead of throwing and stopping application startup

```csharp
public bool? RegisterValidationOptionsAsHealthChecks { get; set; }
```

<a id="validatorlifetime"></a>

## ValidatorLifetime

- Name: `ValidatorLifetime`
- Modifiers: `get; set`
- Returns: `ServiceLifetime`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3345-merge.Sha.a9c0325e0c8c70bc9d0569ac4df1fe5d29ffa4ec/src/Foundation/FoundationOptions.cs)

The Validator lifetime

```csharp
public ServiceLifetime ValidatorLifetime { get; set; }
```
