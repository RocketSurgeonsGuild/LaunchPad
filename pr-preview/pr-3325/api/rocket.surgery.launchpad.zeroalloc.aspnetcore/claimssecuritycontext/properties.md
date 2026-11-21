# ClaimsSecurityContext Properties

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.AspNetCore](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.aspnetcore.md)
- Type: [ClaimsSecurityContext](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/claimssecuritycontext.md)
- Kind: `Properties`
- Members: `3`

A `Claims.ClaimsPrincipal` as a security context.

<a id="claims"></a>

## Claims

- Name: `Claims`
- Modifiers: `get`
- Returns: `IReadOnlyDictionary<string, string>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.b95ef48c1c24b4d5eac70fa83d50ac9f4aff0224/src/ZeroAlloc.AspNetCore/Authorization/HttpSecurityContext.cs#L55)

```csharp
public IReadOnlyDictionary<string, string> Claims { get; }
```

<a id="id"></a>

## Id

- Name: `Id`
- Modifiers: `get`
- Returns: `string`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.b95ef48c1c24b4d5eac70fa83d50ac9f4aff0224/src/ZeroAlloc.AspNetCore/Authorization/HttpSecurityContext.cs#L49)

```csharp
public string Id { get; }
```

<a id="roles"></a>

## Roles

- Name: `Roles`
- Modifiers: `get`
- Returns: `IReadOnlySet<string>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.b95ef48c1c24b4d5eac70fa83d50ac9f4aff0224/src/ZeroAlloc.AspNetCore/Authorization/HttpSecurityContext.cs#L52)

```csharp
public IReadOnlySet<string> Roles { get; }
```
