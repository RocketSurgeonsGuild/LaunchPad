# PolicyOutcome Methods

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Type: [PolicyOutcome](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyoutcome.md)
- Kind: `Methods`
- Members: `7`

What one policy decided: allowed, or denied with the scopes the caller lacks or the policy's own failure.

<a id="deconstruct"></a>
<a id="deconstruct-bool-ireadonlylist-string-authorizationfailure"></a>

## Deconstruct(bool, IReadOnlyList<string>, AuthorizationFailure?)

- Name: `Deconstruct(bool, IReadOnlyList<string>, AuthorizationFailure?)`

```csharp
public struct PolicyOutcome
{
    public void Deconstruct(
        out bool Allowed,
        out IReadOnlyList<string> MissingScopes,
        out AuthorizationFailure? Failure)
    {
        // ...
    }
}
```

## Parameters

- `Allowed` (`bool`)
- `MissingScopes` (`IReadOnlyList<string>`)
- `Failure` (`AuthorizationFailure?`)

<a id="equals"></a>
<a id="equals-object"></a>

## Equals(object)

- Name: `Equals(object)`
- Modifiers: `override`
- Returns: `bool`

Indicates whether this instance and a specified object are equal.

```csharp
public struct PolicyOutcome
{
    public override bool Equals(
        object obj)
    {
        // ...
    }
}
```

## Parameters

- `obj` (`object`)
  The object to compare with the current instance.

## Returns

`bool` -- `true` if `obj` and this instance are the same type and represent the same value; otherwise, `false`.

<a id="equals-policyoutcome"></a>

## Equals(PolicyOutcome)

- Name: `Equals(PolicyOutcome)`
- Returns: `bool`

Indicates whether the current object is equal to another object of the same type.

```csharp
public struct PolicyOutcome
{
    public bool Equals(
        PolicyOutcome other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([PolicyOutcome](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyoutcome.md))
  An object to compare with this object.

## Returns

`bool` -- `true` if the current object is equal to the `other` parameter; otherwise, `false`.

<a id="gethashcode"></a>

## GetHashCode

- Name: `GetHashCode`
- Modifiers: `override`
- Returns: `int`

Returns the hash code for this instance.

```csharp
public struct PolicyOutcome
{
    public override int GetHashCode()
    {
        // ...
    }
}
```

## Returns

`int` -- A 32-bit signed integer that is the hash code for this instance.

<a id="op-equality"></a>
<a id="op-equality-policyoutcome-policyoutcome"></a>

## op_Equality(PolicyOutcome, PolicyOutcome)

- Name: `op_Equality(PolicyOutcome, PolicyOutcome)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public struct PolicyOutcome
{
    public static bool operator ==(
        PolicyOutcome left,
        PolicyOutcome right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([PolicyOutcome](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyoutcome.md))
- `right` ([PolicyOutcome](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyoutcome.md))

<a id="op-inequality"></a>
<a id="op-inequality-policyoutcome-policyoutcome"></a>

## op_Inequality(PolicyOutcome, PolicyOutcome)

- Name: `op_Inequality(PolicyOutcome, PolicyOutcome)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public struct PolicyOutcome
{
    public static bool operator !=(
        PolicyOutcome left,
        PolicyOutcome right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([PolicyOutcome](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyoutcome.md))
- `right` ([PolicyOutcome](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyoutcome.md))

<a id="tostring"></a>

## ToString

- Name: `ToString`
- Modifiers: `override`
- Returns: `string`

Returns the fully qualified type name of this instance.

```csharp
public struct PolicyOutcome
{
    public override string ToString()
    {
        // ...
    }
}
```

## Returns

`string` -- The fully qualified type name.
