# StrongTypeDescription Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3203/api/rocket.surgery.launchpad.primitives.md)
- Type: [StrongTypeDescription](/LaunchPad/pr-preview/pr-3203/api/rocket.surgery.launchpad.primitives/strongtypedescription.md)
- Kind: `Methods`
- Members: `3`

What an integration needs to know about one strong type: its value type, the format it is written in, and the constraints its rules set (lengths, patterns, bounds, allowed values), read from its SuperStrong validators. A rule written as a predicate ( `Satisfies`) is not described.

<a id="accept"></a>
<a id="accept-istrongtypevisitor-tresult"></a>

## Accept(IStrongTypeVisitor<TResult>)

- Name: `Accept(IStrongTypeVisitor<TResult>)`
- Modifiers: `abstract`
- Returns: `TResult`

Hands the strong type and its value to `visitor` as generic arguments.

```csharp
public abstract class StrongTypeDescription
{
    public abstract TResult Accept<TResult>(
        IStrongTypeVisitor<TResult> visitor)
    {
        // ...
    }
}
```

## Parameters

- `visitor` ([IStrongTypeVisitor<TResult>](/LaunchPad/pr-preview/pr-3203/api/rocket.surgery.launchpad.primitives/istrongtypevisitor-1.md))
  The visitor.

## Returns

`TResult` -- What the visitor returned.

<a id="create"></a>
<a id="create-string"></a>

## Create(string?)

- Name: `Create(string?)`
- Modifiers: `static`
- Returns: [StrongTypeDescription](/LaunchPad/pr-preview/pr-3203/api/rocket.surgery.launchpad.primitives/strongtypedescription.md)

Describes a strong type whose value is not a number.

```csharp
public abstract class StrongTypeDescription
{
    public static StrongTypeDescription Create<TStrongType, TPrimitive>(
        string? format = null)
    {
        // ...
    }
}
```

## Parameters

- `format` (`string?`) `optional`
  The format of the value, from [StrongTypeFormatAttribute](/LaunchPad/pr-preview/pr-3203/api/rocket.surgery.launchpad.primitives/strongtypeformatattribute.md).

## Returns

[StrongTypeDescription](/LaunchPad/pr-preview/pr-3203/api/rocket.surgery.launchpad.primitives/strongtypedescription.md) -- The description.

<a id="createnumber"></a>
<a id="createnumber-string"></a>

## CreateNumber(string?)

- Name: `CreateNumber(string?)`
- Modifiers: `static`
- Returns: [StrongTypeDescription](/LaunchPad/pr-preview/pr-3203/api/rocket.surgery.launchpad.primitives/strongtypedescription.md)

Describes a strong type whose value is a number, which also has bounds.

```csharp
public abstract class StrongTypeDescription
{
    public static StrongTypeDescription CreateNumber<TStrongType, TPrimitive>(
        string? format = null)
    {
        // ...
    }
}
```

## Parameters

- `format` (`string?`) `optional`
  The format of the value, from [StrongTypeFormatAttribute](/LaunchPad/pr-preview/pr-3203/api/rocket.surgery.launchpad.primitives/strongtypeformatattribute.md).

## Returns

[StrongTypeDescription](/LaunchPad/pr-preview/pr-3203/api/rocket.surgery.launchpad.primitives/strongtypedescription.md) -- The description.
