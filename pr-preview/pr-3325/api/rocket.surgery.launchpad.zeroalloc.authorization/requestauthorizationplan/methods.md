# RequestAuthorizationPlan Methods

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Type: [RequestAuthorizationPlan](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplan.md)
- Kind: `Methods`
- Members: `3`

The policies a request requires, written by LaunchPad's generator for every request with a policy attribute and registered when its assembly loads.

<a id="attheedge"></a>
<a id="attheedge-isecuritycontext"></a>

## AtTheEdge(ISecurityContext)

- Name: `AtTheEdge(ISecurityContext)`
- Modifiers: `abstract`
- Returns: `ISecurityContext`

The caller as policies see them at the edge, where there is no request yet: reading the request throws.

```csharp
public abstract class RequestAuthorizationPlan
{
    public abstract ISecurityContext AtTheEdge(
        ISecurityContext context)
    {
        // ...
    }
}
```

## Parameters

- `context` (`ISecurityContext`)
  The caller.

<a id="find"></a>
<a id="find-string"></a>

## Find(string)

- Name: `Find(string)`
- Modifiers: `static` `nullable`
- Returns: [RequestAuthorizationPlan?](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplan.md)
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.b95ef48c1c24b4d5eac70fa83d50ac9f4aff0224/src/ZeroAlloc.Authorization/RequestAuthorizationPlan.cs#L101-L105)

The plan of the request with this id, if it has policies.

```csharp
public abstract class RequestAuthorizationPlan
{
    public static RequestAuthorizationPlan? Find(
        string id)
    {
        // ...
    }
}
```

## Parameters

- `id` (`string`)
  The request's full type name.

<a id="register"></a>
<a id="register-policygroup"></a>

## Register(PolicyGroup[])

- Name: `Register(PolicyGroup[])`
- Modifiers: `static`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.b95ef48c1c24b4d5eac70fa83d50ac9f4aff0224/src/ZeroAlloc.Authorization/RequestAuthorizationPlan.cs#L149-L152)

Registers the plan of a request. Called by generated code.

```csharp
public abstract class RequestAuthorizationPlan
{
    public static void Register<TRequest>(
        params PolicyGroup[] groups)
    {
        // ...
    }
}
```

## Parameters

- `groups` (`PolicyGroup[]`)
  The requirements, all of which must pass.
