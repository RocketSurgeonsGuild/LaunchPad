# AuthorizationDecision Methods

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Type: [AuthorizationDecision](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationdecision.md)
- Kind: `Methods`
- Members: `7`

What a request's policies decide for a caller.

<a id="deconstruct"></a>
<a id="deconstruct-problem-ireadonlylist-string"></a>

## Deconstruct(Problem?, IReadOnlyList<string>)

- Name: `Deconstruct(Problem?, IReadOnlyList<string>)`

```csharp
public struct AuthorizationDecision
{
    public void Deconstruct(
        out Problem? Problem,
        out IReadOnlyList<string> RequiredScopes)
    {
        // ...
    }
}
```

## Parameters

- `Problem` (`Problem?`)
- `RequiredScopes` (`IReadOnlyList<string>`)

<a id="equals"></a>
<a id="equals-object"></a>

## Equals(object)

- Name: `Equals(object)`
- Modifiers: `override`
- Returns: `bool`

Indicates whether this instance and a specified object are equal.

```csharp
public struct AuthorizationDecision
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

<a id="equals-authorizationdecision"></a>

## Equals(AuthorizationDecision)

- Name: `Equals(AuthorizationDecision)`
- Returns: `bool`

Indicates whether the current object is equal to another object of the same type.

```csharp
public struct AuthorizationDecision
{
    public bool Equals(
        AuthorizationDecision other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([AuthorizationDecision](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationdecision.md))
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
public struct AuthorizationDecision
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
<a id="op-equality-authorizationdecision-authorizationdecision"></a>

## op_Equality(AuthorizationDecision, AuthorizationDecision)

- Name: `op_Equality(AuthorizationDecision, AuthorizationDecision)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public struct AuthorizationDecision
{
    public static bool operator ==(
        AuthorizationDecision left,
        AuthorizationDecision right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([AuthorizationDecision](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationdecision.md))
- `right` ([AuthorizationDecision](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationdecision.md))

<a id="op-inequality"></a>
<a id="op-inequality-authorizationdecision-authorizationdecision"></a>

## op_Inequality(AuthorizationDecision, AuthorizationDecision)

- Name: `op_Inequality(AuthorizationDecision, AuthorizationDecision)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public struct AuthorizationDecision
{
    public static bool operator !=(
        AuthorizationDecision left,
        AuthorizationDecision right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([AuthorizationDecision](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationdecision.md))
- `right` ([AuthorizationDecision](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationdecision.md))

<a id="tostring"></a>

## ToString

- Name: `ToString`
- Modifiers: `override`
- Returns: `string`

Returns the fully qualified type name of this instance.

```csharp
public struct AuthorizationDecision
{
    public override string ToString()
    {
        // ...
    }
}
```

## Returns

`string` -- The fully qualified type name.
