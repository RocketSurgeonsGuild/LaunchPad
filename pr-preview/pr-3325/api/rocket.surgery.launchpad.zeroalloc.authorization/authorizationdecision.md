# AuthorizationDecision

- Kind: `struct`
- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Version: `2.0.1-pr.1286`
- Namespace: `Rocket.Surgery.LaunchPad.Authorization`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.b95ef48c1c24b4d5eac70fa83d50ac9f4aff0224/src/ZeroAlloc.Authorization/RequestAuthorizer.cs)
- Implements: `IEquatable<AuthorizationDecision>`

What a request's policies decide for a caller.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Authorization;

public sealed readonly struct AuthorizationDecision
    : System.IEquatable<Rocket.Surgery.LaunchPad.Authorization.AuthorizationDecision>
{
    // ...
}
```

## Constructors

- [AuthorizationDecision(Problem?, IReadOnlyList<string>)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationdecision/constructors.md#constructor-problem-ireadonlylist-string) -- What a request's policies decide for a caller.

## Properties

- [Allowed](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationdecision/properties.md#allowed) : [AuthorizationDecision](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationdecision.md) `static` `get` -- The caller is allowed.
- [IsAllowed](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationdecision/properties.md#isallowed) : `bool` `get` -- The caller is allowed.
- [Problem](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationdecision/properties.md#problem) : `Problem?` `get; init` -- Null when allowed; otherwise `Foundation.Unauthorized` or `Foundation.Forbidden`.
- [RequiredScopes](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationdecision/properties.md#requiredscopes) : `IReadOnlyList<string>` `get; init` -- The scopes of the requirement that denied the caller, for the challenge; empty otherwise.

## Methods

- [Deconstruct(Problem?, IReadOnlyList<string>)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationdecision/methods.md#deconstruct-problem-ireadonlylist-string)
- [Equals(object)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationdecision/methods.md#equals-object) : `bool` -- Indicates whether this instance and a specified object are equal.
- [Equals(AuthorizationDecision)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationdecision/methods.md#equals-authorizationdecision) : `bool` -- Indicates whether the current object is equal to another object of the same type.
- [GetHashCode](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationdecision/methods.md#gethashcode) : `int` -- Returns the hash code for this instance.
- [op_Equality(AuthorizationDecision, AuthorizationDecision)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationdecision/methods.md#op-equality-authorizationdecision-authorizationdecision) : `bool` `static`
- [op_Inequality(AuthorizationDecision, AuthorizationDecision)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationdecision/methods.md#op-inequality-authorizationdecision-authorizationdecision) : `bool` `static`
- [ToString](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationdecision/methods.md#tostring) : `string` -- Returns the fully qualified type name of this instance.

## Fields

- [MissingScopesProperty](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationdecision/fields.md#missingscopesproperty) : `string` `static` -- The key of `Problem.Properties` that holds the scopes a forbidden caller lacks, separated by spaces.
