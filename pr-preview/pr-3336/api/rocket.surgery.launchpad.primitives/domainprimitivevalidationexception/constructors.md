# DomainPrimitiveValidationException Constructors

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.primitives.md)
- Type: [DomainPrimitiveValidationException](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.primitives/domainprimitivevalidationexception.md)
- Kind: `Constructors`
- Members: `4`

Thrown when a domain primitive is created from a value its rules reject, unless `LaunchPadPrimitivesExceptionType` names another exception.

<a id="constructor"></a>
<a id="constructor-primitiveerror"></a>

## DomainPrimitiveValidationException(PrimitiveError)

- Name: `Constructor(PrimitiveError)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3336-merge.Sha.7cda342c7a30094b955266549c4928a5c52b02bb/src/Primitives/DomainPrimitiveValidationException.cs#L15)

Creates the exception.

```csharp
public sealed class DomainPrimitiveValidationException
{
    public DomainPrimitiveValidationException(
        PrimitiveError error)
    {
        // ...
    }
}
```

## Parameters

- `error` ([PrimitiveError](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.primitives/primitiveerror.md))
  Why the value was rejected.

<a id="constructor-primitiveerror-string"></a>

## DomainPrimitiveValidationException(PrimitiveError, string)

- Name: `Constructor(PrimitiveError, string)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3336-merge.Sha.7cda342c7a30094b955266549c4928a5c52b02bb/src/Primitives/DomainPrimitiveValidationException.cs#L22)

Creates the exception.

```csharp
public sealed class DomainPrimitiveValidationException
{
    public DomainPrimitiveValidationException(
        PrimitiveError error,
        string paramName)
    {
        // ...
    }
}
```

## Parameters

- `error` ([PrimitiveError](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.primitives/primitiveerror.md))
  Why the value was rejected.
- `paramName` (`string`)
  The parameter that held the value.

<a id="constructor-primitiveerror-exception"></a>

## DomainPrimitiveValidationException(PrimitiveError, Exception?)

- Name: `Constructor(PrimitiveError, Exception?)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3336-merge.Sha.7cda342c7a30094b955266549c4928a5c52b02bb/src/Primitives/DomainPrimitiveValidationException.cs#L29)

Creates the exception.

```csharp
public sealed class DomainPrimitiveValidationException
{
    public DomainPrimitiveValidationException(
        PrimitiveError error,
        Exception? innerException)
    {
        // ...
    }
}
```

## Parameters

- `error` ([PrimitiveError](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.primitives/primitiveerror.md))
  Why the value was rejected.
- `innerException` (`Exception?`)
  What caused it.

<a id="constructor-primitiveerror-exception-string"></a>

## DomainPrimitiveValidationException(PrimitiveError, Exception?, string)

- Name: `Constructor(PrimitiveError, Exception?, string)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3336-merge.Sha.7cda342c7a30094b955266549c4928a5c52b02bb/src/Primitives/DomainPrimitiveValidationException.cs#L38)

Creates the exception.

```csharp
public sealed class DomainPrimitiveValidationException
{
    public DomainPrimitiveValidationException(
        PrimitiveError error,
        Exception? innerException,
        string paramName)
    {
        // ...
    }
}
```

## Parameters

- `error` ([PrimitiveError](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.primitives/primitiveerror.md))
  Why the value was rejected.
- `innerException` (`Exception?`)
  What caused it.
- `paramName` (`string`)
  The parameter that held the value.
