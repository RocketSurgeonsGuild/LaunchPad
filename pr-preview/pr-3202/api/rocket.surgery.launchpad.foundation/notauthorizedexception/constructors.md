# NotAuthorizedException Constructors

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3202/api/rocket.surgery.launchpad.foundation.md)
- Type: [NotAuthorizedException](/LaunchPad/pr-preview/pr-3202/api/rocket.surgery.launchpad.foundation/notauthorizedexception.md)
- Kind: `Constructors`
- Members: `3`

NotAuthorizedException.

<a id="constructor-string"></a>

## NotAuthorizedException(string)

- Name: `Constructor(string)`

Initializes a new instance of the [NotAuthorizedException](/LaunchPad/pr-preview/pr-3202/api/rocket.surgery.launchpad.foundation/notauthorizedexception.md) class.

```csharp
public class NotAuthorizedException
{
    public NotAuthorizedException(
        string message)
    {
        // ...
    }
}
```

## Parameters

- `message` (`string`)
  The message that describes the error.

<a id="constructor-string-exception"></a>

## NotAuthorizedException(string, Exception)

- Name: `Constructor(string, Exception)`

Initializes a new instance of the [NotAuthorizedException](/LaunchPad/pr-preview/pr-3202/api/rocket.surgery.launchpad.foundation/notauthorizedexception.md) class.

```csharp
public class NotAuthorizedException
{
    public NotAuthorizedException(
        string message,
        Exception innerException)
    {
        // ...
    }
}
```

## Parameters

- `message` (`string`)
  The error message that explains the reason for the exception.
- `innerException` (`Exception`)
  The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.

<a id="constructor"></a>

## NotAuthorizedException

- Name: `Constructor`

Initializes a new instance of the [NotAuthorizedException](/LaunchPad/pr-preview/pr-3202/api/rocket.surgery.launchpad.foundation/notauthorizedexception.md) class.

```csharp
public class NotAuthorizedException
{
    public NotAuthorizedException()
    {
        // ...
    }
}
```
