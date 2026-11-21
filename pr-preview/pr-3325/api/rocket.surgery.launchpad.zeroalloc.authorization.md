# Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization

- Package: `Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization`
- Version: `2.0.1-pr.1286`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/tree/Branch.pull-3325-merge.Sha.b95ef48c1c24b4d5eac70fa83d50ac9f4aff0224/src/ZeroAlloc.Authorization)
- NuGet: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](https://www.nuget.org/packages/Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization)

## Namespaces

### Rocket.Surgery.LaunchPad.Authorization

- [AuthorizationBehavior](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationbehavior.md) -- `class` `sealed` -- The ZeroAlloc.Mediator pipeline behavior that checks a request's policies before anything else runs. A caller with no identity answers the `Foundation.Unauthorized` case of the response union, and a caller a policy denies answers `Foundation.Forbidden`; a request without policies passes straight through.
- [AuthorizationDecision](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationdecision.md) -- `struct` `readonly` -- What a request's policies decide for a caller.
- [AuthorizationDeniedException](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationdeniedexception.md) -- `class` `sealed` -- A request was denied where there is no response to answer with: a stream request, or a response that has no case for the denial. It carries the `Foundation.Unauthorized` or `Foundation.Forbidden` problem, which each transport writes as that case.
- [AuthorizationOutcomes](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationoutcomes.md) -- `class` `sealed` -- What each policy decided for the current call, by policy name and arguments. The edge and the mediator pipeline share it through the call's service scope, so a policy runs once.
- [AuthorizationResponse<TResponse>](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationresponse-1.md) -- `class` `static` -- How a denial becomes a response: the conversions of `AuthorizationResponse`1.Unauthorized` and `AuthorizationResponse`1.Forbidden` to the response union, registered by generated code.
- [PolicyEvaluator](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyevaluator.md) -- `delegate` -- Runs one policy for a caller, with the services of the call.
- [PolicyGroup](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policygroup.md) -- `class` `sealed` -- One requirement of a request: a single policy (a `[RequirePolicy]`), or several of which any one is enough (a `[RequireAnyPolicy]`). Every group of a request must pass.
- [PolicyOutcome](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyoutcome.md) -- `struct` `readonly` -- What one policy decided: allowed, or denied with the scopes the caller lacks or the policy's own failure.
- [PolicyRequirement](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyrequirement.md) -- `class` `sealed` -- One policy a request names: its name and arguments, the scopes it declares and how to run it.
- [PolicyResolver](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyresolver.md) -- `class` `static` -- Resolves a policy class for a call. Called by generated code.
- [RequestAuthorizationOptions](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationoptions.md) -- `class` `sealed` -- How LaunchPad reads what a caller was granted.
- [RequestAuthorizationPlan](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplan.md) -- `class` `abstract` -- The policies a request requires, written by LaunchPad's generator for every request with a policy attribute and registered when its assembly loads.
- [RequestAuthorizationPlan<TRequest>](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplan-1.md) -- `class` `sealed` -- The plan of `TRequest`.
- [RequestAuthorizer](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizer.md) -- `class` `static` -- Decides a request's plan for a caller, policy by policy, recording each outcome in the call's [AuthorizationOutcomes](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationoutcomes.md) and reusing the ones already there. The mediator pipeline and the edge both decide through it.
- [RequiresScopesAttribute](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requiresscopesattribute.md) -- `class` `sealed` -- On a `[Policy]` class: the scopes the policy requires of the caller, all of them. LaunchPad checks them before the policy runs, and publishes them as the scopes of every operation whose request names the policy.

### Rocket.Surgery.LaunchPad.Authorization.Joints

- [ZeroAllocAuthorizationJoint](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/zeroallocauthorizationjoint.md) -- `class` -- Registers what the [AuthorizationBehavior](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationbehavior.md) shares with the edge: the [AuthorizationOutcomes](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationoutcomes.md) of each call and the [RequestAuthorizationOptions](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationoptions.md). Implements the `Joints.IServiceJoint`
