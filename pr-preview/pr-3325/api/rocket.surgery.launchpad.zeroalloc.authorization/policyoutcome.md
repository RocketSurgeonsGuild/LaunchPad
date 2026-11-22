# PolicyOutcome

- Kind: `struct`
- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Version: `2.0.1-pr.1294`
- Namespace: `Rocket.Surgery.LaunchPad.Authorization`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.b12891a76083b718e1ed50b524f1c30a2c5447d7/src/ZeroAlloc.Authorization/AuthorizationOutcomes.cs)
- Implements: `IEquatable<PolicyOutcome>`

What one policy decided: allowed, or denied with the scopes the caller lacks or the policy's own failure.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Authorization;

public sealed readonly struct PolicyOutcome
    : System.IEquatable<Rocket.Surgery.LaunchPad.Authorization.PolicyOutcome>
{
    // ...
}
```

## Constructors

- [PolicyOutcome(bool, IReadOnlyList<string>, AuthorizationFailure?)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyoutcome/constructors.md#constructor-bool-ireadonlylist-string-authorizationfailure) -- What one policy decided: allowed, or denied with the scopes the caller lacks or the policy's own failure.

## Properties

- [Allowed](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyoutcome/properties.md#allowed) : `bool` `get; init` -- The policy allows the caller.
- [Failure](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyoutcome/properties.md#failure) : `AuthorizationFailure?` `get; init` -- The policy's own failure, when its logic denied the caller.
- [MissingScopes](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyoutcome/properties.md#missingscopes) : `IReadOnlyList<string>` `get; init` -- The declared scopes the caller lacks; empty when the policy's own logic decided.
- [Success](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyoutcome/properties.md#success) : [PolicyOutcome](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyoutcome.md) `static` `get` -- The policy allows the caller.

## Methods

- [Deconstruct(bool, IReadOnlyList<string>, AuthorizationFailure?)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyoutcome/methods.md#deconstruct-bool-ireadonlylist-string-authorizationfailure)
- [Equals(object)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyoutcome/methods.md#equals-object) : `bool` -- Indicates whether this instance and a specified object are equal.
- [Equals(PolicyOutcome)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyoutcome/methods.md#equals-policyoutcome) : `bool` -- Indicates whether the current object is equal to another object of the same type.
- [GetHashCode](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyoutcome/methods.md#gethashcode) : `int` -- Returns the hash code for this instance.
- [op_Equality(PolicyOutcome, PolicyOutcome)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyoutcome/methods.md#op-equality-policyoutcome-policyoutcome) : `bool` `static`
- [op_Inequality(PolicyOutcome, PolicyOutcome)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyoutcome/methods.md#op-inequality-policyoutcome-policyoutcome) : `bool` `static`
- [ToString](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyoutcome/methods.md#tostring) : `string` -- Returns the fully qualified type name of this instance.
