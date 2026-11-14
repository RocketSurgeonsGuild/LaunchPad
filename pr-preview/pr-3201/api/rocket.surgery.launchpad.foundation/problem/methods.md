# Problem Methods

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3201/api/rocket.surgery.launchpad.foundation.md)
- Type: [Problem](/LaunchPad/pr-preview/pr-3201/api/rocket.surgery.launchpad.foundation/problem.md)
- Kind: `Methods`
- Members: `7`

The base of LaunchPad's standard error cases: what a handler returns, as a case of its result union, instead of throwing. Each transport turns a case into its own error shape (HTTP ProblemDetails, a gRPC status, a GraphQL error) from [IProblemDetailsData](/LaunchPad/pr-preview/pr-3201/api/rocket.surgery.launchpad.foundation/iproblemdetailsdata.md).

<a id="clone"></a>

## <Clone>$

- Name: `<Clone>$`
- Modifiers: `abstract`
- Returns: [Problem](/LaunchPad/pr-preview/pr-3201/api/rocket.surgery.launchpad.foundation/problem.md)

```csharp
public record Problem
{
    public abstract Problem <Clone>$()
    {
        // ...
    }
}
```

<a id="equals"></a>
<a id="equals-object"></a>

## Equals(object?)

- Name: `Equals(object?)`
- Modifiers: `override`
- Returns: `bool`

Determines whether the specified object is equal to the current object.

```csharp
public record Problem
{
    public override bool Equals(
        object? obj)
    {
        // ...
    }
}
```

## Parameters

- `obj` (`object?`)
  The object to compare with the current object.

## Returns

`bool` -- `true` if the specified object is equal to the current object; otherwise, `false`.

<a id="equals-problem"></a>

## Equals(Problem?)

- Name: `Equals(Problem?)`
- Modifiers: `virtual`
- Returns: `bool`

Indicates whether the current object is equal to another object of the same type.

```csharp
public record Problem
{
    public virtual bool Equals(
        Problem? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([Problem?](/LaunchPad/pr-preview/pr-3201/api/rocket.surgery.launchpad.foundation/problem.md))
  An object to compare with this object.

## Returns

`bool` -- `true` if the current object is equal to the `other` parameter; otherwise, `false`.

<a id="gethashcode"></a>

## GetHashCode

- Name: `GetHashCode`
- Modifiers: `override`
- Returns: `int`

Serves as the default hash function.

```csharp
public record Problem
{
    public override int GetHashCode()
    {
        // ...
    }
}
```

## Returns

`int` -- A hash code for the current object.

<a id="op-equality"></a>
<a id="op-equality-problem-problem"></a>

## op_Equality(Problem?, Problem?)

- Name: `op_Equality(Problem?, Problem?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public record Problem
{
    public static bool operator ==(
        Problem? left,
        Problem? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Problem?](/LaunchPad/pr-preview/pr-3201/api/rocket.surgery.launchpad.foundation/problem.md))
- `right` ([Problem?](/LaunchPad/pr-preview/pr-3201/api/rocket.surgery.launchpad.foundation/problem.md))

<a id="op-inequality"></a>
<a id="op-inequality-problem-problem"></a>

## op_Inequality(Problem?, Problem?)

- Name: `op_Inequality(Problem?, Problem?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public record Problem
{
    public static bool operator !=(
        Problem? left,
        Problem? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Problem?](/LaunchPad/pr-preview/pr-3201/api/rocket.surgery.launchpad.foundation/problem.md))
- `right` ([Problem?](/LaunchPad/pr-preview/pr-3201/api/rocket.surgery.launchpad.foundation/problem.md))

<a id="tostring"></a>

## ToString

- Name: `ToString`
- Modifiers: `override`
- Returns: `string`

Returns a string that represents the current object.

```csharp
public record Problem
{
    public override string ToString()
    {
        // ...
    }
}
```

## Returns

`string` -- A string that represents the current object.
